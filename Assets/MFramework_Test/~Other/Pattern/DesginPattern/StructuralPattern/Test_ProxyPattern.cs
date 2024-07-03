using MFramework;
using UnityEngine;

public class Test_ProxyPattern : MonoBehaviour
{
    private void Start()
    {
        Image image = new ProxyImage("Test.jpg");//��ʱ�������
        image.Display();//���μ���
        image.Display();//����Ҫ�ٴμ���
    }

    public class ProxyImage : Image
    {
        private RealImage realImage;
        private string fileName;

        public ProxyImage(string fileName)
        {
            this.fileName = fileName;
        }

        public void Display()
        {
            if (realImage == null)
            {
                realImage = new RealImage(fileName);
            }
            realImage.Display();
        }
    }

    public interface Image
    {
        void Display();
    }
    public class RealImage : Image
    {
        private string fileName;

        public RealImage(string fileName)
        {
            this.fileName = fileName;
            Load(fileName);
        }

        public void Display()
        {
            MLog.Print($"Image:{fileName}");
        }

        private void Load(string fileName)
        {
            MLog.Print("Load From Disk...");
        }
    }
}
