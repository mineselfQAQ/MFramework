using MFramework;
using System;
using UnityEngine;

public class Test_Bit : MonoBehaviour
{
    private void Start()
    {
        string bStr1 = "11111111";
        byte b1 = Convert.ToByte(bStr1, 2);
        byte b2 = 0b11111111;//Tip:0bxxx所表示的是一个常量(无符号数)
        //sbyte b2 = 0b11111111;//错误，0b11111111指代的就是255
        string bStr2 = "11111111";//在有符号中得-1---为补码11111111，这等价于反码11111110,也等价于原码10000001
        sbyte b3 = Convert.ToSByte(bStr2, 2);
        MLog.Print(b1);//255
        MLog.Print(b2);//255
        MLog.Print(b3);//-1


        string iStr1 = "11111111111111111111111111111111";
        uint i1 = Convert.ToUInt32(iStr1, 2);
        MLog.Print(i1);

        //操作---取最低位，因为1其实就是0001，所以使用&运算符的话高位都会得0，只保留下最低位之间的运算
        MLog.Print(0b1001 & 1);
    }
}
