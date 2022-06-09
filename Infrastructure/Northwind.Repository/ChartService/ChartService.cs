using Northwind.Contracts;
using Northwind.Contracts.ServiceChart;
using Northwind.Entities.DataTransferObject;
using Northwind.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Repository.CartService
{
    public class ChartService : IChartService
    {
        private readonly IRepositoryManager _repository;

        public ChartService(IRepositoryManager repository)
        {
            _repository = repository;
        }

        public Tuple<int, IEnumerable<Product>, string> GetAllProduct(bool trackChanges)
        {
            IEnumerable<Product> products1 = null;

            try
            {
                IEnumerable<Product> products = _repository.Products.GetAllProduct(trackChanges: false);

                return Tuple.Create(1, products, "success");
            }
            catch (Exception ex)
            {
                return Tuple.Create(-1, products1, ex.Message);
            }
        }

        public OrderDetail AddToChart(int productId, int quantity, string customerID)
        {
            Order order = new Order();
            Product product = new Product();
            OrderDetail orderDetail = new OrderDetail();
            Customer customer = new Customer();

            try
            {
                product = _repository.Products.GetProduct(productId, trackChanges: false);
                order = _repository.Orders.GetAllOrder(trackChanges: true).Where(c => c.CustomerId == customerID && c.ShippedDate == null).SingleOrDefault();

                //ngecek apakah order = null, ketika customer ada dan shippedDate = nul
                if (order == null)
                {
                    order = new Order();
                    order.CustomerId = customerID;
                    order.OrderDate = DateTime.Now;
                    _repository.Orders.CreateOrder(order);
                    _repository.Save();
                }

                orderDetail = _repository.OrderDetails.GetOrderDetail(order.OrderId, productId, trackChanges: true);

                if (orderDetail == null)
                {
                    //orderid, productid ada tpi shippedDate = null, maka buat orderDetails
                    orderDetail = new OrderDetail();
                    orderDetail.OrderId = order.OrderId;
                    orderDetail.ProductId = productId;
                    orderDetail.UnitPrice = (decimal)((decimal)product.UnitPrice * quantity);
                    orderDetail.Quantity = (short)quantity;
                    //orderDetail.Discount = (real)product.Discontinued;

                    _repository.OrderDetails.CreateOrderDetail(orderDetail);
                    _repository.Save();
                }

                else
                {
                    //dijalankan ketika quantity = 0, tpi kalo masukin order ga mungkin jumlah itu 0
                    if (orderDetail.Quantity == null)
                    {
                        orderDetail.Quantity = (short)quantity;
                        orderDetail.UnitPrice = (decimal)((decimal)product.UnitPrice * quantity);
                    }
                    else
                    {
                        orderDetail.Quantity += (short)quantity;
                        orderDetail.UnitPrice += (decimal)((decimal)product.UnitPrice * quantity);
                    }

                    _repository.OrderDetails.UpdateOrderDetail(orderDetail);
                    _repository.Save();
                }

                return orderDetail;
            }
            catch (Exception ex)
            {
                return orderDetail;
            }
        }

        public Tuple<int, Order, string> Checkout(int id)
        {
            Order orderResult = new Order();

            try
            {
                var order = _repository.Orders.GetOrder(id, trackChanges: true);

                //ngecek kondisi dimana order, yg diinput kosong atau engga
                if (order == null)
                {
                    return Tuple.Create(-1, order, "Order Id is not found");
                }

                order = new Order();
                order.RequiredDate = DateTime.Now;

                //Ngecek dan nampilin dengan kondisi : apakah id yg diinput sama kyk id di orderDetail, lalu ditampung di orderDetails tipe data List.
                List<OrderDetail> orderDetails = _repository.OrderDetails.GetAllOrderDetail(trackChanges: true).Where(c => c.OrderId == id).ToList();

                //ngeluarin data dari list orderDetails
                foreach (var productList in orderDetails)
                {
                    //Get idProduct sesuai dengan orderDetail
                    var product = _repository.Products.GetProduct(productList.ProductId, trackChanges: true);
                    //Ngurangin jumlah stock product, dikurangin dari quantity jumlah yg dimasukin ke dalam orderDetails
                    //product.UnitsInStock -= productList.Quantity;
                    product.UnitsInStock = (short?)(product.UnitsInStock - productList.Quantity);

                    _repository.Products.UpdateProduct(product);
                    _repository.Save();
                }

                _repository.Orders.UpdateOrder(order);
                _repository.Save();
                return Tuple.Create(1, order, "Success");
            }
            catch (Exception ex)
            {
                return Tuple.Create(-1, orderResult, ex.Message);
            }
        }

        public Order ShipOrder(ShipDto shipDto, int id)
        {
            throw new NotImplementedException();
        }

        /*public Order ShipOrder(ShipDto shipDto, int id)
         {
             Order newOrder = new Order();

             try
             {
                 var order = _repository.Orders.GetOrder(id, trackChanges: true);
                 var customer = _repository.Customers.GetCustomerAsync(order.CustomerId, trackChanges: false);

                 order.ShipAddress = customer.Address;
                 order.ShipCity = customer.City;
                 order.ShipRegion = customer.Region;
                 order.ShipPostalCode = customer.PostalCode;
                 order.ShipCountry = customer.Country;
                 order.ShipVia = shipDto.ShipVIa;
                 order.Freight = shipDto.Freight;
                 order.ShipName = shipDto.ShipName;
                 order.ShippedDate = shipDto.ShippedDate;

                 _repository.Orders.UpdateOrder(order);
                 _repository.Save();

                 return order;
             }
             catch (Exception ex)
             {
                 return newOrder;
             }
         }*/
    }
}
