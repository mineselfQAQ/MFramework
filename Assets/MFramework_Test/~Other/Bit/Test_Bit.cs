using MFramework;
using System;
using UnityEngine;

public class Test_Bit : MonoBehaviour
{
    private void Start()
    {
        string bStr1 = "11111111";
        byte b1 = Convert.ToByte(bStr1, 2);
        byte b2 = 0b11111111;//Tip:0bxxx����ʾ����һ������(�޷�����)
        //sbyte b2 = 0b11111111;//����0b11111111ָ���ľ���255
        string bStr2 = "11111111";//���з����е�-1---Ϊ����11111111����ȼ��ڷ���11111110,Ҳ�ȼ���ԭ��10000001
        sbyte b3 = Convert.ToSByte(bStr2, 2);
        MLog.Print(b1);//255
        MLog.Print(b2);//255
        MLog.Print(b3);//-1


        string iStr1 = "11111111111111111111111111111111";
        uint i1 = Convert.ToUInt32(iStr1, 2);
        MLog.Print(i1);

        //����---ȡ���λ����Ϊ1��ʵ����0001������ʹ��&������Ļ���λ�����0��ֻ���������λ֮�������
        MLog.Print(0b1001 & 1);
    }
}
