using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace MFramework.Text
{
    public enum MTextTypewriterMode
    {
        Off,
        Show,
        Fade,
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(TMP_Text))]
    [AddComponentMenu("MFramework/MText/Text Animator")]
    public sealed class MTextAnimator : MonoBehaviour
    {
        [SerializeField] private MTextTypewriterMode typewriterMode = MTextTypewriterMode.Fade;
        [SerializeField] private bool playOnEnable = true;
        [SerializeField] private float charactersPerSecond = 24f;
        [SerializeField] private bool playInlineEffects = true;
        [SerializeField] private UnityEvent typewriterFinished;

        private TMP_Text _text;
        private Coroutine _routine;
        private IReadOnlyList<MTextInlineEffectRange> _effects = Array.Empty<MTextInlineEffectRange>();
        private TMP_MeshInfo[] _cachedMeshInfo;
        private bool _finishEventInvoked;
        private bool _typewriterForceCompleted;
        private float _elapsed;

        public bool IsPlaying { get; private set; }
        public UnityEvent TypewriterFinished => typewriterFinished;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            if (playOnEnable)
            {
                RebuildAndPlay(_effects);
            }
        }

        private void OnDisable()
        {
            Stop();
        }

        public void RebuildAndPlay(IReadOnlyList<MTextInlineEffectRange> effects)
        {
            _effects = effects ?? Array.Empty<MTextInlineEffectRange>();
            if (!isActiveAndEnabled) return;

            Stop();
            _routine = StartCoroutine(PlayRoutine());
        }

        public void PlayText()
        {
            RebuildAndPlay(_effects);
        }

        public void FinishTextImmediately()
        {
            if (!IsPlaying) return;

            _typewriterForceCompleted = true;
            if (_cachedMeshInfo == null)
            {
                PrepareMesh();
            }

            ApplyFrame(_elapsed, true);
            PushMesh();
            InvokeFinishedOnce();

            if (!ShouldKeepInlineEffectsPlaying())
            {
                Stop();
            }
        }

        public void Stop()
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }

            IsPlaying = false;
        }

        private IEnumerator PlayRoutine()
        {
            PrepareMesh();

            IsPlaying = true;
            _finishEventInvoked = false;
            _typewriterForceCompleted = false;
            _elapsed = 0f;

            float elapsed = 0f;
            int characterCount = GetCharacterCount();
            float cps = Mathf.Max(1f, charactersPerSecond);
            float typewriterDuration = typewriterMode == MTextTypewriterMode.Off ? 0f : characterCount / cps;

            while (IsPlaying)
            {
                bool typewriterComplete = _typewriterForceCompleted || elapsed >= typewriterDuration;
                _elapsed = elapsed;
                ApplyFrame(elapsed, typewriterComplete);
                PushMesh();

                if (typewriterComplete)
                {
                    InvokeFinishedOnce();
                    if (!ShouldKeepInlineEffectsPlaying())
                    {
                        break;
                    }
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            IsPlaying = false;
            _routine = null;
        }

        private void PrepareMesh()
        {
            EnsureText();
            _text.ForceMeshUpdate(true, true);
            _cachedMeshInfo = _text.textInfo.CopyMeshInfoVertexData();
        }

        private void ApplyFrame(float elapsed, bool typewriterComplete)
        {
            RestoreCachedMesh();
            TMP_TextInfo textInfo = _text.textInfo;

            if (typewriterMode != MTextTypewriterMode.Off && !typewriterComplete)
            {
                ApplyTypewriter(textInfo, elapsed);
            }

            if (playInlineEffects)
            {
                ApplyInlineEffects(textInfo, elapsed);
            }
        }

        private void RestoreCachedMesh()
        {
            if (_cachedMeshInfo == null) return;

            TMP_TextInfo textInfo = _text.textInfo;
            for (int i = 0; i < textInfo.meshInfo.Length && i < _cachedMeshInfo.Length; i++)
            {
                Array.Copy(_cachedMeshInfo[i].vertices, textInfo.meshInfo[i].vertices, _cachedMeshInfo[i].vertices.Length);
                Array.Copy(_cachedMeshInfo[i].colors32, textInfo.meshInfo[i].colors32, _cachedMeshInfo[i].colors32.Length);
            }
        }

        private void ApplyTypewriter(TMP_TextInfo textInfo, float elapsed)
        {
            float visibleProgress = elapsed * Mathf.Max(1f, charactersPerSecond);
            int visibleIndex = 0;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                float charProgress = Mathf.Clamp01(visibleProgress - visibleIndex);
                if (typewriterMode == MTextTypewriterMode.Show)
                {
                    SetCharacterAlpha(textInfo, charInfo, charProgress >= 1f ? 1f : 0f);
                }
                else
                {
                    SetCharacterAlpha(textInfo, charInfo, charProgress);
                }

                visibleIndex++;
            }
        }

        private void ApplyInlineEffects(TMP_TextInfo textInfo, float elapsed)
        {
            if (textInfo.characterCount == 0) return;

            float effectTime = Mathf.Repeat(elapsed, 1024f);
            for (int i = 0; i < _effects.Count; i++)
            {
                MTextInlineEffectRange effect = _effects[i];
                int start = Mathf.Clamp(effect.StartIndex, 0, textInfo.characterCount - 1);
                int end = Mathf.Clamp(effect.EndIndex, start, textInfo.characterCount - 1);

                for (int charIndex = start; charIndex <= end; charIndex++)
                {
                    TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];
                    if (!charInfo.isVisible) continue;

                    switch (effect.Type)
                    {
                        case MTextInlineEffectType.Wave:
                            OffsetCharacter(textInfo, charInfo, Vector3.up * Mathf.Sin(effectTime * 8f + charIndex * 0.45f) * 8f);
                            break;
                        case MTextInlineEffectType.Shake:
                            float x = Mathf.Sin(effectTime * 46f + charIndex * 11f) * 1.8f;
                            float y = Mathf.Cos(effectTime * 41f + charIndex * 7f) * 1.8f;
                            OffsetCharacter(textInfo, charInfo, new Vector3(x, y, 0f));
                            break;
                        case MTextInlineEffectType.Pulse:
                            ScaleCharacter(textInfo, charInfo, 1f + Mathf.Sin(effectTime * 7f + charIndex * 0.3f) * 0.12f);
                            break;
                        case MTextInlineEffectType.Color:
                            SetCharacterColor(textInfo, charInfo, effect.Color);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        private static void OffsetCharacter(TMP_TextInfo textInfo, TMP_CharacterInfo charInfo, Vector3 offset)
        {
            Vector3[] vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            int vertexIndex = charInfo.vertexIndex;
            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;
        }

        private static void ScaleCharacter(TMP_TextInfo textInfo, TMP_CharacterInfo charInfo, float scale)
        {
            Vector3[] vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            int vertexIndex = charInfo.vertexIndex;
            Vector3 center = (vertices[vertexIndex + 0] + vertices[vertexIndex + 2]) * 0.5f;

            for (int i = 0; i < 4; i++)
            {
                int index = vertexIndex + i;
                vertices[index] = center + (vertices[index] - center) * scale;
            }
        }

        private static void SetCharacterAlpha(TMP_TextInfo textInfo, TMP_CharacterInfo charInfo, float alpha)
        {
            Color32[] colors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;
            int vertexIndex = charInfo.vertexIndex;
            byte a = (byte)(Mathf.Clamp01(alpha) * 255f);

            for (int i = 0; i < 4; i++)
            {
                colors[vertexIndex + i].a = a;
            }
        }

        private static void SetCharacterColor(TMP_TextInfo textInfo, TMP_CharacterInfo charInfo, Color color)
        {
            Color32[] colors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;
            int vertexIndex = charInfo.vertexIndex;
            Color32 c = color;

            for (int i = 0; i < 4; i++)
            {
                c.a = colors[vertexIndex + i].a;
                colors[vertexIndex + i] = c;
            }
        }

        private void PushMesh()
        {
            TMP_TextInfo textInfo = _text.textInfo;
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                meshInfo.mesh.colors32 = meshInfo.colors32;
                _text.UpdateGeometry(meshInfo.mesh, i);
            }
        }

        private int GetCharacterCount()
        {
            TMP_TextInfo textInfo = _text.textInfo;
            int count = 0;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                if (textInfo.characterInfo[i].isVisible)
                {
                    count++;
                }
            }

            return count;
        }

        private void InvokeFinishedOnce()
        {
            if (_finishEventInvoked) return;

            _finishEventInvoked = true;
            typewriterFinished?.Invoke();
        }

        private bool ShouldKeepInlineEffectsPlaying()
        {
            return playInlineEffects && _effects.Count > 0;
        }

        private void EnsureText()
        {
            if (_text == null)
            {
                _text = GetComponent<TMP_Text>();
            }
        }
    }
}
