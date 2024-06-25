using TMPro;
using UnityEngine;

namespace MFramework
{
    public class MTextData
    {
        public MTextCharData[] charData;//Tip:不带上空格(如："I'm ok"是5个字符而非6个字符)

        public void ConstructCharData(TMP_Text text)
        {
            TMP_TextInfo info = text.textInfo;
            //Tip：meshInfo[0]为主要文本信息，[1][2]可能指的是阴影/描边之类的内容
            int n = info.meshInfo[0].vertexCount / 4;//文字个数
            charData = new MTextCharData[n];
            int count = 0;
            MTextCharData data = new MTextCharData();
            data.vertices = new Vector3[4];
            data.oVertices = new Vector3[4];
            data.colors32 = new Color32[4];
            data.oColors32 = new Color32[4];

            for (int i = 0; i < info.meshInfo[0].vertexCount; i++)
            {
                data.vertices[i % 4] = info.meshInfo[0].vertices[i];
                data.oVertices[i % 4] = info.meshInfo[0].vertices[i];

                data.colors32[i % 4] = text.color.ToColor32();
                data.oColors32[i % 4] = text.color.ToColor32();

                //每轮的最后一个顶点需要补全数据以及初始化下一个data
                if (i % 4 == 3)
                {
                    data.center = new Vector3((info.meshInfo[0].vertices[i - 1].x + info.meshInfo[0].vertices[i - 3].x) / 2,
                    (info.meshInfo[0].vertices[i - 1].y + info.meshInfo[0].vertices[i - 3].y) / 2);
                    data.index = count;
                    charData[count++] = data;

                    data = new MTextCharData();
                    data.vertices = new Vector3[4];
                    data.oVertices = new Vector3[4];
                    data.colors32 = new Color32[4];
                    data.oColors32 = new Color32[4];
                }
            }
        }
    }
}