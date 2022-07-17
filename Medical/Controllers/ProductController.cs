using Medical.Models;
using Medical.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Medical.Controllers
{
    public class ProductController : Controller
    {
        private readonly MedicalContext _medicalContext;
        private IWebHostEnvironment environment;
        public ProductController(MedicalContext medicalContext, IWebHostEnvironment myEnvironment)
        {
            _medicalContext = medicalContext;
            environment = myEnvironment;
        }
        //歷史訂單
        //預設會員19 需要抓登入資料
        //id=memberID
        public IActionResult OrderList(int? id = 19)
        {

            IEnumerable<OrderDetailViewModel> list = null;
            if (id != 0)
            {
                list = _medicalContext.Orders.Where(a => a.MemberId == id)
                    .Select(a => new OrderDetailViewModel
                    {
                        Order = a,
                        Member=a.Member,
                        Orderstate=a.OrderState,
                        Paytype=a.PayType,
                        ShipType=a.ShipType
                    });

            }
            return View(list);
        }


        //關於訂單內產品新增產品評論
        //先秀出訂單明細表 
        //id=orderID
        public IActionResult OrderDetailList(int? id)
        {
            IEnumerable<OrderDetailViewModel> list = null;
            if (id != 0)
            {
                list = _medicalContext.OrderDetails.Where(a => a.OrderId == id)
                    .Select(a=>new OrderDetailViewModel
                        {
                            OrderDetail=a,
                            Order=a.Order,
                            Product=a.Product,
                            Member=a.Order.Member,
                            
                        });
              
            }
            return View(list);
        }

        //根據訂單產品ID 新增評論 (前台)
        public IActionResult createReview(int? id)
        {
            var p = _medicalContext.Reviews.FirstOrDefault(a => a.ProductId == id);

                return View(p);
        }
        [HttpPost]
        public IActionResult createReview(Review addReviewView)
        {


            _medicalContext.Reviews.Add(addReviewView);
            _medicalContext.SaveChanges();
            return RedirectToAction("OrderDetailList");
        }

        //管理產品評論 (後台)
        public IActionResult ReviewList()
        {
            IEnumerable < CReviewViewModel > list= null;
            list = _medicalContext.Reviews.Select(p => new CReviewViewModel
            {
                Review = p,
                Member = p.Member,
                RatingType = p.RatingType,             
            }) ;
            

            return View(list);
        }




        // ============ 柏鈞 =================
        public IActionResult productList()
        {
            return View(GetProducts(1));
        }
        [HttpPost]
        public IActionResult productList(int currentPageIndex)
        {
            return View(GetProducts(currentPageIndex));
        }


        private CProductForShowViewModel GetProducts(int currentPage)
        {
            int maxRows = 8;
            CProductForShowViewModel prodModel = new CProductForShowViewModel()
            {
                productList = _medicalContext.Products.ToList(),
                brandList = _medicalContext.ProductBrands.ToList(),
                cateList = _medicalContext.ProductCategories.ToList(),
                prodSpec = _medicalContext.ProductSpecifications.ToList()

            };

            prodModel.prodSpec = (from prod in this._medicalContext.ProductSpecifications
                                  select prod)
                        //.Where(p=>p.Product.ProductBrand.ProductBrandName == "雷朋") 條件
                        .OrderBy(prod => prod.Product.ProductName)
                        .Skip((currentPage - 1) * maxRows)
                        .Take(maxRows).ToList();

            double pageCount = (double)((decimal)this._medicalContext.Products.Count() / Convert.ToDecimal(maxRows));
            prodModel.PageCount = (int)Math.Ceiling(pageCount);

            prodModel.CurrentPageIndex = currentPage;

            return prodModel;
        }
        public IActionResult ProductDetail(string productName)
        {
            Product p = _medicalContext.Products.FirstOrDefault(p => p.ProductName == productName);

            if (p == null)
            {
                return RedirectToAction("productList");
            }


            ProductSpecification ps = _medicalContext.ProductSpecifications.FirstOrDefault(
                ps => ps.Product.ProductName == productName);
            List<Review> reviewList = _medicalContext.Reviews.Where(rw => rw.Product.ProductName == productName).ToList();
            List<Member> memList = _medicalContext.Members.ToList();
            List<RatingType> ratings = _medicalContext.RatingTypes.ToList();
            List<ProductBrand> brandList = _medicalContext.ProductBrands.ToList();
            List<ProductCategory> cateList = _medicalContext.ProductCategories.ToList();


            CShoppingCartViewModel cartView = new CShoppingCartViewModel()
            {
                prodReviewList = reviewList,
                prod = p,
                prodSpec = ps,
                ratingList = ratings,
                memList = memList,
                brandList = brandList,
                cateList = cateList

            };

            return View(cartView);
        }

        [HttpPost]
        public IActionResult AddToCart(CAddToCartViewModel AddToCartvModel)
        {
            Product prod = _medicalContext.Products.FirstOrDefault(p => p.ProductId == AddToCartvModel.txtPId);
            if (prod == null)
                return RedirectToAction("productList");

            ShoppingCart cart = new ShoppingCart()
            {
                MemberId = 1,
                ProductId = AddToCartvModel.txtPId,
                ProductAmount = AddToCartvModel.txtCount
            };

            _medicalContext.Add(cart);
            _medicalContext.SaveChanges();

            return RedirectToAction("productList");
        }

        public IActionResult CartViewList(int? id)
        {
            List<ShoppingCart> cartList = _medicalContext.ShoppingCarts.Where(c => c.MemberId == 1).ToList();

            List<Product> prodList = _medicalContext.Products.ToList();
            List<ProductSpecification> prodspecList = _medicalContext.ProductSpecifications.ToList();

            List<CShoppingCartItem> cartForShowList = new List<CShoppingCartItem>();

            Product p = new Product();

            foreach (ShoppingCart cart in cartList)
            {
                CShoppingCartItem item = new CShoppingCartItem();
                item.cart = cart;
                item.prod = prodList.FirstOrDefault(p => p.ProductId == cart.ProductId);
                item.prodspec = prodspecList.FirstOrDefault(ps => ps.ProductId == cart.ProductId);
                cartForShowList.Add(item);
            }

            return View(cartForShowList);

        }
        [HttpPost]
        public IActionResult CartViewList(int ShoppingCartId)
        {
            ShoppingCart cart = _medicalContext.ShoppingCarts.FirstOrDefault(c => c.ShoppingCartId == ShoppingCartId);

            _medicalContext.Remove(cart);
            _medicalContext.SaveChanges();
            return RedirectToAction("CartViewList");
        }


        public IActionResult CheckViewList(int? id)
        {

            List<ShoppingCart> cartList = _medicalContext.ShoppingCarts.Where(c => c.MemberId == 1).ToList();

            List<Product> prodList = _medicalContext.Products.ToList();



            List<ProductSpecification> prodspecList = _medicalContext.ProductSpecifications.ToList();


            List<CShoppingCartItem> checkForShowList = new List<CShoppingCartItem>();

            Product p = new Product();

            foreach (ShoppingCart cart in cartList)
            {
                CShoppingCartItem item = new CShoppingCartItem();
                item.cart = cart;
                item.prod = prodList.FirstOrDefault(p => p.ProductId == cart.ProductId);
                item.prodspec = prodspecList.FirstOrDefault(ps => ps.ProductId == cart.ProductId);
                checkForShowList.Add(item);
            }


            return View(checkForShowList);
        }


        public IActionResult QueryOrder(int? id)
        {
            List<Order> orderList = _medicalContext.Orders.Where(o => o.MemberId == 2).OrderByDescending(o => o.OrderDate).ToList();
            List<OrderDetail> orderDetailList = null;

            foreach (var o in orderList)
            {
                orderDetailList = _medicalContext.OrderDetails.Where(od => od.OrderId == o.OrderId).ToList();
                o.OrderDetails = orderDetailList;
            }


            List<Product> prodList = _medicalContext.Products.ToList();
            List<ProductSpecification> prodspecList = _medicalContext.ProductSpecifications.ToList();



            COrderforShowViewModel cOrderforShowViewModel = new COrderforShowViewModel()
            {

                orderDetailList = orderDetailList,
                orderList = orderList,
                productList = prodList,
                productSpecificationList = prodspecList
            };

            return View(cOrderforShowViewModel);
        }

        //========================= 臨時用=========================

        public IActionResult tempList()
        {
            IEnumerable<MProductViewModel> datas = null;
            datas = _medicalContext.ProductSpecifications.Include(a => a.Product).Include(a => a.Product.ProductBrand)
               .Include(a => a.Product.ProductCategory).Select(a => new MProductViewModel
               {
                   product = a.Product,
                   productBrand = a.Product.ProductBrand,
                   productCate = a.Product.ProductCategory,
                   productSpec = a

               });
            return View(datas);
        }

        public ActionResult tempEdit(int? id)
        {
            IEnumerable<MProductViewModel> datas = _medicalContext.ProductSpecifications.Select(a => new MProductViewModel { product = a.Product, productSpec = a, productCate = a.Product.ProductCategory, productBrand = a.Product.ProductBrand });
            MProductViewModel mProduct = datas.FirstOrDefault(a => a.ProductId == id);
            if (mProduct == null)
                return RedirectToAction("tempList");

            else
            {



                IEnumerable<SelectListItem> objselectListItem = (from p in _medicalContext.ProductBrands
                                                                 where p.ProductBrandName != null
                                                                 select p).ToList().Select(p => new SelectListItem
                                                                 { Value = p.ProductBrandId.ToString(), Text = p.ProductBrandName });

                ViewBag.SelectListItem = objselectListItem;



                return View(mProduct);
            }
        }

        [HttpPost]
        public ActionResult tempEdit(MProductViewModel mp/*, int ProductBrandId*/)
        {


            Product mprod = _medicalContext.Products.FirstOrDefault(a => a.ProductId == mp.productSpec.ProductId);
            ProductSpecification mps = _medicalContext.ProductSpecifications.FirstOrDefault(m => m.ProductSpecificationId == mp.productSpec.ProductSpecificationId);

            if (mprod != null && mps != null)
            {
                if (mp.photo != null)
                {
                    string mpName = Guid.NewGuid().ToString() + ".jpg";
                    mp.photo.CopyTo(new FileStream(environment.WebRootPath + "/images/" + mpName, FileMode.Create));
                    mps.ProductImage = mpName;
                }
                //Product



                mprod.ProductBrandId = mp.ProductBrandId;
                mprod.ProductName = mp.ProductName;
                mprod.Shelfdate = mp.Shelfdate;
                mprod.Stock = mp.Stock;
                mprod.Discontinued = mp.Discontinued;

                // ProductSpec
                mps.ProductAppearance = mp.ProductAppearance;
                mps.ProductColor = mp.ProductColor;
                mps.ProductMaterial = mp.ProductMaterial;
                mps.UnitPrice = mp.UnitPrice;

            }

            try
            {
                _medicalContext.SaveChanges();
            }
            catch (DbUpdateException)
            {

            }
            return RedirectToAction("tempList");


        }





        public IActionResult tempAddProduct()
        {
            return View();
        }
        [HttpPost]
        public IActionResult tempAddProduct(CProductViewModel cc)
        {

            Product p = new Product()
            {

                ProductBrandId = cc.ProductBrandId,
                ProductCategoryId = cc.ProductCategoryId,
                ProductName = cc.ProductName,
                Discontinued = cc.Discontinued,
                Shelfdate = cc.Shelfdate,
                Stock = cc.Stock


            };
            _medicalContext.Products.Add(p);
            _medicalContext.SaveChanges();

            ProductSpecification ps = new ProductSpecification()
            {


                ProductColor = cc.ProductColor,
                ProductAppearance = cc.ProductAppearance,
                ProductMaterial = cc.ProductMaterial,
                UnitPrice = cc.UnitPrice,
                ProductId = p.ProductId

            };

            if (cc.photo != null)
            {
                string mpName = Guid.NewGuid().ToString() + ".jpg";
                cc.photo.CopyTo(new FileStream(environment.WebRootPath + "/images/" + mpName, FileMode.Create));
                ps.ProductImage = mpName;
            }


            _medicalContext.ProductSpecifications.Add(ps);

            _medicalContext.SaveChanges();

            return RedirectToAction("tempAddProduct");
        }


        public IActionResult aa() {

            return View();
        }

        // ============ 柏鈞 End =================
    }
}
