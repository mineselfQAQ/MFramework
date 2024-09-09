using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogController : MonoBehaviour
{
    protected DialogPanel panel;

    protected void Start()
    {
        panel = UIController.Instance.GetDialogPanel();
    }

    public void ShowDialog()
    {
        panel.OpenSelf();
        //panel.RefreshView();
    }

    public void HideDialog()
    {
        panel.CloseSelf();
    }
}
