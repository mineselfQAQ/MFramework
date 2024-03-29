using MFramework;
using System.Text;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string s = "192.168.50.12";
        byte[] bytes = MSerializationUtility.ByteSerialize(s);
        string str = Encoding.ASCII.GetString(bytes);
        byte[] newBytes = Encoding.ASCII.GetBytes(str);
        object obj = MSerializationUtility.ByteDeserialize(newBytes);
        Debug.Log((string)obj); 

        //byte[] bytes = new byte[] { 1, 2, 3, 4, 5, 6 };
        //string str = Encoding.UTF8.GetString(bytes);
        //byte[] newBytes = Encoding.UTF8.GetBytes(str);

        Debug.Log("");
    }
}
