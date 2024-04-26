using MFramework;
using System.Collections.Generic;
using UnityEngine;

public class Test_CommandPattern : MonoBehaviour
{
    private void Start()
    {
        Stock stock = new Stock();

        Broker broker = new Broker();
        broker.TakeOrder(new BuyStock(stock));
        broker.TakeOrder(new SellStock(stock));
        broker.PlaceOrders();
    }

    public class Broker
    {
        private List<Order> orderList = new List<Order>();

        public void TakeOrder(Order order)
        {
            orderList.Add(order);
        }

        public void PlaceOrders()
        {
            foreach (var order in orderList)
            {
                order.Execute();
            }
            orderList.Clear();
        }
    }

    public class Stock
    {
        private string name = "Good1";
        private int quantity = 10;

        public void Buy()
        {
            MLog.Print($"{name} bought {quantity}");
        }

        public void Sell()
        {
            MLog.Print($"{name} sold {quantity}");
        }
    }

    public interface Order
    {
        void Execute();
    }
    public class BuyStock : Order
    {
        private Stock stock;

        public BuyStock(Stock stock)
        {
            this.stock = stock;
        }

        public void Execute()
        {
            stock.Buy();
        }
    }
    public class SellStock : Order
    {
        private Stock stock;

        public SellStock(Stock stock)
        {
            this.stock = stock;
        }

        public void Execute()
        {
            stock.Sell();
        }
    }
}
