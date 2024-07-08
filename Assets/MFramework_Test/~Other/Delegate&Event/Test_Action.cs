using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Test_Action : MonoBehaviour
{
    public TMP_Text p1HP;
    public TMP_Text p2HP;

    private Player p1;
    private Player p2;

    private void Start()
    {
        p1 = new Player(hp:5);
        p2 = new Player(hp:10);

        UpdateHpText(p1HP, p1.hp);
        UpdateHpText(p2HP, p2.hp);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ActionExecute.ExecutePressE();
            UpdateHpText(p1HP, p1.hp);
            UpdateHpText(p2HP, p2.hp);
        }
    }

    private void UpdateHpText(TMP_Text text, int hp)
    {
        text.text = $"HP: {hp}";
    }

    public class ActionExecute
    {
        public static List<IOnPressE> onPressEList = new List<IOnPressE>();

        public static void ExecutePressE()
        {
            foreach (var item in onPressEList)
            {
                item.OnPressE();
            }
        }
    }

    public class Player : IOnPressE
    {
        public Player(int hp)
        {
            this.hp = hp;
            Register();
        }

        public int hp;

        public void OnPressE()
        {
            hp += 5;
        }

        public void Register()
        {
            ActionExecute.onPressEList.Add(this);
        }
    }

    public interface IOnPressE : IEventBase
    {
        void OnPressE();
    }
}