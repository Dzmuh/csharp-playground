﻿// © Корпорация Майкрософт (Microsoft Corp.).  Все права защищены.
// Этот код выпущен на условиях 
// открытой лицензии Майкрософт (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using SampleSupport;
using System.Xml.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using QuerySamples;

namespace DataSetSampleQueries
{
    public static class CustomSequenceOperators
    {
        public static IEnumerable<S> Combine<S>(this IEnumerable<DataRow> first, IEnumerable<DataRow> second, System.Func<DataRow, DataRow, S> func)
        {
            using (IEnumerator<DataRow> e1 = first.GetEnumerator(), e2 = second.GetEnumerator())
            {
                while (e1.MoveNext() && e2.MoveNext())
                {
                    yield return func(e1.Current, e2.Current);
                }
            }
        }
    }


    [Title("Примеры запросов 101 LINQ для наборов данных")]
    [Prefix("DataSetLinq")]
    class DataSetLinqSamples : SampleHarness
    {
        private DataSet testDS;

        public DataSetLinqSamples() {
            testDS = TestHelper.CreateTestDataset();       
        }

#region "Операторы ограничений"

        [Category("Операторы ограничений")]
        [Title("Where - Простой 1")]
        [Description("Этот пример используется там, где нужно найти все элементы массива со значениями меньше 5.")]
        public void DataSetLinq1() {

            var numbers = testDS.Tables["Numbers"].AsEnumerable();

            var lowNums = from n in numbers
                where n.Field<int>("number") < 5
                select n;

            Console.WriteLine("Numbers < 5:");
            foreach (var x in lowNums) {
                Console.WriteLine(x[0]);
            }
        }

        [Category("Операторы ограничений")]
        [Title("Where - Простой 2")]
        [Description("Образец использования предложения Where для поиска всех продуктов, отсутствующих в запасе.")]
        public void DataSetLinq2() {

            var products = testDS.Tables["Products"].AsEnumerable();

            var soldOutProducts = from p in products
                where p.Field<int>("UnitsInStock") == 0
                select p;
    
            Console.WriteLine("Sold out products:");
            foreach (var product in soldOutProducts) {
                Console.WriteLine(product.Field<string>("ProductName") + " is sold out!");
            }
        }

        [Category("Операторы ограничений")]
        [Title("Where - Простой 3")]
        [Description("Этот пример используется там, где нужно найти все продукты на складе " +
                     "с ценой более 3,00 за единицу.")]
        public void DataSetLinq3() {

            var products = testDS.Tables["Products"].AsEnumerable();

            var expensiveInStockProducts = from p in products
                where p.Field<int>("UnitsInStock") > 0 
                where p.Field<decimal>("UnitPrice") > 3.00M
                select p;
    
            Console.WriteLine("In-stock products that cost more than 3.00:");
            foreach (var product in expensiveInStockProducts) {
                Console.WriteLine(product.Field<string>("ProductName") + " is in stock and costs more than 3.00.");
            }
        }

        [Category("Операторы ограничений")]
        [Title("Where - углубленная детализация")]
        [Description("В этом примере для поиска всех клиентов из Вашингтона используется предложение Where, " +
                     "после чего полученная последовательность используется для углубленной детализации до уровня их " +
                     "заказов.")]
        public void DataSetLinq4() {

            var customers = testDS.Tables["Customers"].AsEnumerable();

            var waCustomers =
                from c in customers
                where c.Field<string>("Region") == "WA"
                select c;
    
            Console.WriteLine("Customers from Washington and their orders:");
            foreach (var customer in waCustomers) {
                Console.WriteLine("Customer {0}: {1}", customer.Field<string>("CustomerID"), customer["CompanyName"]);
                foreach (var order in customer.GetChildRows("CustomersOrders")) {
                    Console.WriteLine("  Order {0}: {1}", order["OrderID"], order["OrderDate"]);
                }
            }

            
        }

        [Category("Операторы ограничений")]
        [Title("Where - индексированное")]
        [Description("В этом примере показано индексированное предложение Where, которое возвращает цифры, имя которых " +
                    "короче своего значения.")]
        public void DataSetLinq5() {
            
            var digits = testDS.Tables["Digits"].AsEnumerable();

            var shortDigits = digits.Where((digit, index) => digit.Field<string>(0).Length < index);

            Console.WriteLine("Short digits:");
            foreach (var d in shortDigits) {
                Console.WriteLine("The word " + d["digit"] + " is shorter than its value.");
            }
        }
        
        [Category("Операторы ограничений")]
        [Title("Single - простой")]
        [Description("Преобразует последовательность в единый результат")]
        public void DataSetLinq106() {

            //Создание таблицы с одной строкой
            var singleRowTable = new DataTable("SingleRowTable");
            singleRowTable.Columns.Add("id", typeof(int));
            singleRowTable.Rows.Add(new object[] {1});

            var singleRow = singleRowTable.AsEnumerable().Single();

            Console.WriteLine(singleRow != null);            
        }

        [Category("Операторы ограничений")]
        [Title("Single - с предикатом")]
        [Description("Возвращает один элемент на основании заданного предиката")]
        public void DataSetLinq107() {

            //Создание таблицы с двумя строками
            var table = new DataTable("MyTable");
            table.Columns.Add("id", typeof(int));
            table.Rows.Add(new object[] {1});
            table.Rows.Add(new object[] {2});

            var singleRow = table.AsEnumerable().Single(r => r.Field<int>("id") == 1);

            Console.WriteLine(singleRow != null);          
        }

#endregion

#region "Операторы проекции"

        [Category("Операторы проекции")]
        [Title("Select - простой 1")]
        [Description("Образец использования Select для получения последовательности целых чисел, на единицу превышающих " +
                     "целые числа в существующем массиве.")]
        public void DataSetLinq6() {

            var numbers = testDS.Tables["Numbers"].AsEnumerable();

            var numsPlusOne =
                from n in numbers
                select n.Field<int>(0) + 1;
 
            Console.WriteLine("Numbers + 1:");
            foreach (var i in numsPlusOne) {
                Console.WriteLine(i);
            }
        }

        [Category("Операторы проекции")]
        [Title("Select - простой 2")]
        [Description("Образец использования Select для возвращения только имен из списка продуктов.")]
        public void DataSetLinq7() {

            var products = testDS.Tables["Products"].AsEnumerable();

            var productNames =
                from p in products
                select p.Field<string>("ProductName");
            
            Console.WriteLine("Product Names:");
            foreach (var productName in productNames) {
                Console.WriteLine(productName);
            }           
        }

        [Category("Операторы проекции")]
        [Title("Select - преобразование")]
        [Description("Образец использования Select для получения последовательности строк, представляющих " +
                     "текстовый вариант последовательности целых чисел.")]
        public void DataSetLinq8() {

            var numbers = testDS.Tables["Numbers"].AsEnumerable();
            string[] strings = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

            var textNums = numbers.Select(p => strings[p.Field<int>("number")]);

            Console.WriteLine("Number strings:");
            foreach (var s in textNums) {
                Console.WriteLine(s);
            }          
        }

        [Category("Операторы проекции")]
        [Title("Select - анонимные типы 1")]
        [Description("Образец использования Select для получения последовательности в верхнем " +
                     "и в нижнем регистре каждого слова в исходном массиве.")]
        public void DataSetLinq9() {

            var words = testDS.Tables["Words"].AsEnumerable();

            var upperLowerWords = words.Select(p => new {Upper = (p.Field<string>(0)).ToUpper(), 
                Lower = (p.Field<string>(0)).ToLower()});

            foreach (var ul in upperLowerWords) {
                Console.WriteLine("Uppercase: "+  ul.Upper + ", Lowercase: " + ul.Lower);
            }
        }

        [Category("Операторы проекции")]
        [Title("Select - анонимные типы 2")]
        [Description("Образец использования Select для получения последовательности, содержащей текстовые " +
                     "представления цифр, а также оценку, является ли их длина четной или нечетной.")]
        public void DataSetLinq10() {

            var numbers = testDS.Tables["Numbers"].AsEnumerable();
            var digits = testDS.Tables["Digits"];
            string[] strings = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

            var digitOddEvens = numbers.
                Select(n => new {Digit = digits.Rows[n.Field<int>("number")]["digit"], 
                    Even = (n.Field<int>("number") % 2 == 0)});

            foreach (var d in digitOddEvens) {
                Console.WriteLine("The digit {0} is {1}.", d.Digit, d.Even ? "even" : "odd");
            }
        }

        [Category("Операторы проекции")]
        [Title("Select - анонимные типы 3")]
        [Description("Образец использования Select для получения последовательности, содержащей некоторые свойства " +
                     "продуктов, включая значение UnitPrice, переименованное в Price " +
                     "в результирующем типе.")]
        public void DataSetLinq11() {
            
            var products = testDS.Tables["Products"].AsEnumerable();

            var productInfos = products.
                Select(n => new {ProductName = n.Field<string>("ProductName"), 
                    Category = n.Field<string>("Category"), Price = n.Field<decimal>("UnitPrice")});

            Console.WriteLine("Product Info:");
            foreach (var productInfo in productInfos) {
                Console.WriteLine("{0} is in the category {1} and costs {2} per unit.", productInfo.ProductName, productInfo.Category, productInfo.Price);
            }        
        } 


        [Category("Операторы проекции")]
        [Title("Select - с индексом")]
        [Description("Образец использования индексированного предложения Select для определения соответствия значения целых чисел " +
                     "в массиве их позиции в этом массиве.")]
        public void DataSetLinq12() {

            var numbers = testDS.Tables["Numbers"].AsEnumerable();

            var numsInPlace = numbers.Select((num, index) => new {Num = num.Field<int>("number"), 
                InPlace = (num.Field<int>("number") == index)});

            Console.WriteLine("Number: In-place?");
            foreach (var n in numsInPlace) {
                Console.WriteLine("{0}: {1}", n.Num, n.InPlace);
            }
        }

        [Category("Операторы проекции")]
        [Title("Select - с фильтром")]
        [Description("В этом примере объединяются предложения Select и Where, чтобы создать простой запрос, который возвращает " +
                     "текстовое представление каждой цифры, меньшей 5.")]
        public void DataSetLinq13() {

            var numbers = testDS.Tables["Numbers"].AsEnumerable();
            var digits = testDS.Tables["Digits"];

            var lowNums =
                from n in numbers
                where n.Field<int>("number") < 5
                select digits.Rows[n.Field<int>("number")].Field<string>("digit");
    
            Console.WriteLine("Numbers < 5:");
            foreach (var num in lowNums) {
                Console.WriteLine(num);
            } 
        }

        [Category("Операторы проекции")]
        [Title("SelectMany - составное предложение From 1")]
        [Description("В этом примере используется составное предложение From, чтобы создать запрос, возвращающий все пары " +
                     "чисел из обоих массивов, в каждой их которых число из массива numbersA меньше числа " +
                     "из numbersB.")]
        public void DataSetLinq14() {
            
            var numbersA = testDS.Tables["NumbersA"].AsEnumerable();
            var numbersB = testDS.Tables["NumbersB"].AsEnumerable();

            var pairs =
                from a in numbersA
                from b in numbersB
                where a.Field<int>("number") < b.Field<int>("number")
                select new {a = a.Field<int>("number")  , b = b.Field<int>("number")};
 
            Console.WriteLine("Pairs where a < b:");
            foreach (var pair in pairs) {
                Console.WriteLine("{0} is less than {1}", pair.a, pair.b);
            }    
        }

        [Category("Операторы проекции")]
        [Title("SelectMany - составное предложение From 2")]
        [Description("Образец использования составного предложения From для выбора всех заказов, в которых " +
                     "сумма заказа меньше 500,00.")]
        public void DataSetLinq15() {

            var customers = testDS.Tables["Customers"].AsEnumerable();
            var orders = testDS.Tables["Orders"].AsEnumerable();

            var smallOrders = 
                from c in customers
                from o in orders
                where c.Field<string>("CustomerID") == o.Field<string>("CustomerID")
                    && o.Field<decimal>("Total") < 500.00M
                select new {CustomerID = c.Field<string>("CustomerID"), OrderID = o.Field<int>("OrderID"), 
                    Total = o.Field<decimal>("Total")};

            ObjectDumper.Write(smallOrders);
        }

        [Category("Операторы проекции")]
        [Title("SelectMany - составное предложение From 3")]
        [Description("Образец использования составного предложения From для выбора всех заказов, в которых " +
                     "заказ сделан в 1998 г. или позднее.")]
        public void DataSetLinq16() {

            var customers = testDS.Tables["Customers"].AsEnumerable();
            var orders = testDS.Tables["Orders"].AsEnumerable();

            var myOrders =
                from c in customers
                from o in orders
                where c.Field<string>("CustomerID") == o.Field<string>("CustomerID") &&
                o.Field<DateTime>("OrderDate") >= new DateTime(1998, 1, 1)
                select new {CustomerID = c.Field<string>("CustomerID"), OrderID = o.Field<int>("OrderID"), 
                    OrderDate = o.Field<DateTime>("OrderDate")};

            ObjectDumper.Write(myOrders);
        }

        [Category("Операторы проекции")]
        [Title("SelectMany - назначение From")]
        [Description("Образец использования составного предложения From для выбора всех заказов, в которых " +
                     "общая сумма заказа превышает 2000,00, а предложение From используется во избежание " +
                     "повторного запроса итога.")]
        public void DataSetLinq17() {

            var customers = testDS.Tables["Customers"].AsEnumerable();
            var orders = testDS.Tables["Orders"].AsEnumerable();

            var myOrders =
                from c in customers
                from o in orders
                let total = o.Field<decimal>("Total")
                where c.Field<string>("CustomerID") == o.Field<string>("CustomerID") 
                    && total >= 2000.0M
                select new {CustomerID = c.Field<string>("CustomerID"), OrderID = o.Field<int>("OrderID"), total};

            ObjectDumper.Write(myOrders);
        }

        [Category("Операторы проекции")]
        [Title("SelectMany - множественные предложения From")]
        [Description("Образец использования нескольких предложений From, чтобы фильтрация по клиентам могла " +
                     "быть выполнена до выбора их заказов.  Это делает запрос более эффективным, поскольку не требуются " +
                     "не выбираются, а затем отбрасываются заказы для клиентов не из Вашингтона.")]
        public void DataSetLinq18() {

            var customers = testDS.Tables["Customers"].AsEnumerable();
            var orders = testDS.Tables["Orders"].AsEnumerable();
            DateTime cutoffDate = new DateTime(1997, 1, 1);

            var myOrders =
                from c in customers
                where c.Field<string>("Region") == "WA"
                from o in orders
                where c.Field<string>("CustomerID") == o.Field<string>("CustomerID") 
                && (DateTime) o["OrderDate"] >= cutoffDate
                select new {CustomerID = c.Field<string>("CustomerID"), OrderID = o.Field<int>("OrderID")};

            ObjectDumper.Write(myOrders);
        }

        [Category("Операторы проекции")]
        [Title("SelectMany - индексированное")]
        [Description("Образец использования индексированного предложения SelectMany для выбора всех заказов, " +
                     "ссылаясь на клиентов по заказам, в которых они возвращаются " +
                     "из запроса.")]
        public void DataSetLinq19() {

            var customers = testDS.Tables["Customers"].AsEnumerable();
            var orders = testDS.Tables["Orders"].AsEnumerable();

            var customerOrders =
                customers.SelectMany(
                    (cust, custIndex) =>
                    orders.Where(o => cust.Field<string>("CustomerID") == o.Field<string>("CustomerID"))  
                        .Select(o => new {CustomerIndex = custIndex + 1, OrderID = o.Field<int>("OrderID")}));

            foreach(var c in customerOrders) {
                Console.WriteLine("Customer Index: " + c.CustomerIndex +
                                    " has an order with OrderID " + c.OrderID);
            }
        }

#endregion

#region "Операторы секционирования"

        [Category("Операторы секционирования")]
        [Title("Оператор Take - простой")]
        [Description("Образец использования Take для получения только первых 3-х элементов " +
                     "массива.")]
        public void DataSetLinq20() {

            var numbers = testDS.Tables["Numbers"].AsEnumerable();

            var first3Numbers = numbers.Take(3);
    
            Console.WriteLine("First 3 numbers:");
            foreach (var n in first3Numbers) {
                Console.WriteLine(n.Field<int>("number"));
            }       
        }

        [Category("Операторы секционирования")]
        [Title("Оператор Take - вложенный")]
        [Description("Образец использования Take для получения первых 3-х заказов от клиентов " +
                     "в Вашингтоне.")]
        public void DataSetLinq21() {

            var customers = testDS.Tables["Customers"].AsEnumerable();
            var orders = testDS.Tables["Orders"].AsEnumerable();

            var first3WAOrders = (
                from c in customers
                from o in orders
                where c.Field<string>("CustomerID") == o.Field<string>("CustomerID") 
                     && c.Field<string>("Region") == "WA"
                select new {CustomerID = c.Field<string>("CustomerID"), OrderID = o.Field<int>("OrderID"), 
                    OrderDate = o.Field<DateTime>("OrderDate")} ).Take(3);

            Console.WriteLine("First 3 orders in WA:");
            foreach (var order in first3WAOrders) {
                ObjectDumper.Write(order);
            }
        }

        [Category("Операторы секционирования")]
        [Title("Skip - простое")]
        [Description("Образец использования Skip для получения всех, кроме первых 4-х элементов " +
                     "массива.")]
        public void DataSetLinq22() {
            
            var numbers = testDS.Tables["Numbers"].AsEnumerable();

            var allButFirst4Numbers = numbers.Skip(4);
            
            Console.WriteLine("All but first 4 numbers:");
            foreach (var n in allButFirst4Numbers) {
                Console.WriteLine(n.Field<int>("number"));
            }        
        }

        [Category("Операторы секционирования")]
        [Title("Skip - вложенное")]
        [Description("Образец использования Skip для получения всех, кроме первых 2-х заказов от клиентов " +
                     "в Вашингтоне.")]
        public void DataSetLinq23() {

            var customers = testDS.Tables["Customers"].AsEnumerable();
            var orders = testDS.Tables["Orders"].AsEnumerable();

            var allButFirst2Orders = (
                from c in customers
                from o in orders
                where c.Field<string>("CustomerID") == o.Field<string>("CustomerID") 
                    && c.Field<string>("Region") == "WA"
                select new {CustomerID = c.Field<string>("CustomerID"), OrderID = o.Field<int>("OrderID"), 
                    OrderDate = o.Field<DateTime>("OrderDate")} ).Skip(2);

            Console.WriteLine("All but first 2 orders in WA:");
            foreach (var order in allButFirst2Orders) {
                ObjectDumper.Write(order);
            }            
        }    

        [Category("Операторы секционирования")]
        [Title("TakeWhile - простой")]
        [Description("Образец использования TakeWhile для возврата элементов с " +
                     "начала массива до номера, не меньшего 6.")]
        public void DataSetLinq24() {

            var numbers = testDS.Tables["Numbers"].AsEnumerable();

            var firstNumbersLessThan6 = numbers.TakeWhile(n => n.Field<int>("number") < 6);
    
            Console.WriteLine("First numbers less than 6:");
            foreach (var n in firstNumbersLessThan6) {
                Console.WriteLine(n.Field<int>("number"));
            }       
        }
        
        [Category("Операторы секционирования")]
        [Title("TakeWhile - индексированный")]
        [Description("Образец использования TakeWhile для возврата элементов с " +
                    "начала массива, пока не будет найдено число, меньшее, чем его позиция " +
                    "в массиве.")]
        public void DataSetLinq25() {

            var numbers = testDS.Tables["Numbers"].AsEnumerable();

            var firstSmallNumbers = numbers.TakeWhile((n, index) => n.Field<int>("number") >= index);
            
            Console.WriteLine("First numbers not less than their position:");
            foreach (var n in firstSmallNumbers) {
                Console.WriteLine(n.Field<int>("number"));
            }      
        }

        [Category("Операторы секционирования")]
        [Title("SkipWhile - простой")]
        [Description("Образец использования SkipWhile для получения элементов массива, " +
                    "начиная с первого элемента, кратного 3.")]
        public void DataSetLinq26() {

            var numbers = testDS.Tables["Numbers"].AsEnumerable();

            var allButFirst3Numbers = numbers.SkipWhile(n => n.Field<int>("number") % 3 != 0);
    
            Console.WriteLine("All elements starting from first element divisible by 3:");
            foreach (var n in allButFirst3Numbers) {
                Console.WriteLine(n.Field<int>("number"));
            }          
        }

        [Category("Операторы секционирования")]
        [Title("SkipWhile - индексированный")]
        [Description("Образец использования SkipWhile для получения элементов массива, " +
                    "начиная с первого элемента, меньшего своей позиции.")]
        public void DataSetLinq27() {

            var numbers = testDS.Tables["Numbers"].AsEnumerable();

            var laterNumbers = numbers.SkipWhile((n, index) => n.Field<int>("number") >= index);
    
            Console.WriteLine("All elements starting from first element less than its position:");
            foreach (var n in laterNumbers) {
                Console.WriteLine(n.Field<int>("number"));
            }        
        }


#endregion

#region "Операторы упорядочения"

        [Category("Операторы упорядочения")]
        [Title("OrderBy - простой 1")]
        [Description("Пример использования orderby для сортировки списка слов в алфавитном порядке.")]
        public void DataSetLinq28() {

            var words = testDS.Tables["Words"].AsEnumerable();

            var sortedWords =
                from w in words
                orderby w.Field<string>("word")
                select w;
    
            Console.WriteLine("The sorted list of words:");
            foreach (var w in sortedWords) {
                Console.WriteLine(w.Field<string>("word"));
            }
        }

        [Category("Операторы упорядочения")]
        [Title("OrderBy - простой 2")]
        [Description("Пример использования orderby для сортировки списка слов по длине.")]
        public void DataSetLinq29() {

            var words = testDS.Tables["Words"].AsEnumerable();

            var sortedWords =
                from w in words
                orderby w.Field<string>("word").Length
                select w;
    
            Console.WriteLine("The sorted list of words (by length):");
            foreach (var w in sortedWords) {
                Console.WriteLine(w.Field<string>("word"));
            }

            
        }

        [Category("Операторы упорядочения")]
        [Title("OrderBy - простой 3")]
        [Description("Пример использования orderby для сортировки списка продуктов по названию.")]
        public void DataSetLinq30() {

            var products = testDS.Tables["Products"].AsEnumerable();

            var sortedProducts =
                from p in products
                orderby p.Field<string>("ProductName")
                select p.Field<string>("ProductName");

            ObjectDumper.Write(sortedProducts);           
        }

        private class CaseInsensitiveComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return string.Compare(x, y, true);
            }
        }

        [Category("Операторы упорядочения")]
        [Title("OrderBy - сравнение")]
        [Description("Образец использования предложения OrderBy с пользовательским методом сравнения для " +
                     "сортировки слов в массиве без учета регистра.")]
        [LinkedClass("CaseInsensitiveComparer")]
        public void DataSetLinq31() {

            var words3 = testDS.Tables["Words3"].AsEnumerable();

            var sortedWords = words3.OrderBy(a => a.Field<string>("word"), new CaseInsensitiveComparer());

            foreach (var dr in sortedWords)
            {
                Console.WriteLine(dr.Field<string>("word"));
            }
        }

        [Category("Операторы упорядочения")]
        [Title("OrderByDescending - простой 1")]
        [Description("Пример использования orderby и descending для сортировки списка " +
                     "чисел с типом double от наибольшего к наименьшему.")]
        public void DataSetLinq32() {

            var doubles = testDS.Tables["Doubles"].AsEnumerable();

            var sortedDoubles =
                from d in doubles
                orderby d.Field<double>("double") descending
                select d.Field<double>("double");
    
            Console.WriteLine("The doubles from highest to lowest:");
            foreach (var d in sortedDoubles) {
                Console.WriteLine(d);
            }    
        }

        [Category("Операторы упорядочения")]
        [Title("OrderByDescending - простой 2")]
        [Description("Пример использования orderby для сортировки списка продуктов по их числу на складе " +
                     "от наибольшего к наименьшему.")]
        public void DataSetLinq33() {

            var products = testDS.Tables["Products"].AsEnumerable();

            var sortedProducts =
                from p in products
                orderby p.Field<int>("UnitsInStock") descending
                select p.Field<int>("UnitsInStock") ;

            ObjectDumper.Write(sortedProducts);           
        }

        [Category("Операторы упорядочения")]
        [Title("OrderByDescending - сравнение")]
        [Description("Образец использования предложения OrderBy с пользовательским методом сравнения для " +
                     "сортировки слов в массиве по убыванию с учетом регистра.")]
        [LinkedClass("CaseInsensitiveComparer")]
        public void DataSetLinq34() {
    
            var words3 = testDS.Tables["Words3"].AsEnumerable();

            var sortedWords = words3.OrderByDescending(a => a.Field<string>("word"), new CaseInsensitiveComparer());
        
            foreach (var dr in sortedWords)
            {
                Console.WriteLine(dr.Field<string>("word"));
            }   
        }

        [Category("Операторы упорядочения")]
        [Title("ThenBy - простой")]
        [Description("Пример использования составного orderby для сортировки списка цифр, " +
                     "сначала по длине их имени, а затем в алфавитном порядке собственно по имени.")]
        public void DataSetLinq35() {

            var digits = testDS.Tables["Digits"].AsEnumerable();

            var sortedDigits =
                from d in digits 
                orderby d.Field<string>("digit").Length, d.Field<string>("digit")[0]
                select d.Field<string>("digit");

            Console.WriteLine("Sorted digits by Length then first character:");
            foreach (var d in sortedDigits) {
                Console.WriteLine(d);
            }    
        }

        [Category("Операторы упорядочения")]
        [Title("ThenBy - сравнение")]
        [Description("Образец использования OrderBy и ThenBy с пользовательским средством сравнения для " +
                     "сортировки сначала по длине слова, а затем для сортировки слов в массиве без учета регистра.")]
        [LinkedClass("CaseInsensitiveComparer")]
        public void DataSetLinq36() {

            var words3 = testDS.Tables["Words3"].AsEnumerable();

            var sortedWords =
                words3.OrderBy(a => a.Field<string>("word").Length)
                .ThenBy(a => a.Field<string>("word"), new CaseInsensitiveComparer());
        
            foreach (var dr in sortedWords) {
                Console.WriteLine(dr["word"]);
            }
        }

        [Category("Операторы упорядочения")]
        [Title("ThenByDescending - простой")]
        [Description("Образец использования orderby для сортировки списка продуктов " +
                     "сначала по категориям, а затем по цене за единицу, от самой высокой до самой низкой.")]
        public void DataSetLinq37() {

            var products = testDS.Tables["Products"].AsEnumerable();

            var sortedProducts =
                from p in products
                orderby p.Field<string>("Category"), p.Field<decimal>("UnitPrice") descending
                select p;

            foreach (var dr in sortedProducts) {
                Console.WriteLine(dr.Field<string>("ProductName"));
            }
        }

        [Category("Операторы упорядочения")]
        [Title("ThenByDescending - сравнение")]
        [Description("Образец использования OrderBy и ThenBy с пользовательским средством сравнения для " +
                     "сортировки сначала по длине слова, а затем для сортировки с учетом регистра по убыванию " +
                     "слов в массиве.")]
        [LinkedClass("CaseInsensitiveComparer")]
        public void DataSetLinq38() {

            var words3 = testDS.Tables["Words3"].AsEnumerable();

            var sortedWords =
                words3.OrderBy(a => a.Field<string>("word").Length)
                .ThenByDescending(a => a.Field<string>("word"), new CaseInsensitiveComparer());
        
            foreach (var dr in sortedWords){
                Console.WriteLine(dr.Field<string>("word"));
            }
        }

        [Category("Операторы упорядочения")]
        [Title("Reverse")]
        [Description("Образец использования Reverse для создания списка всех цифр в массиве, у которых " +
                     "вторая буква \"i\", в обратном порядка по сравнению с исходным массивом.")]
        public void DataSetLinq39() {

            var digits = testDS.Tables["Digits"].AsEnumerable();

            var reversedIDigits = (
                from d in digits
                where d.Field<string>("digit")[1] == 'i'
                select d).Reverse();
    
            Console.WriteLine("A backwards list of the digits with a second character of 'i':");
            foreach (var d in reversedIDigits) {
                Console.WriteLine(d.Field<string>("digit"));
            }  
        }

        [Category("Операторы группировки")]
        [Title("GroupBy - простой 1")]
        [Description("Образец использования Group By для разбиения списка чисел по " +
                    "их остатку от деления на 5.")]
        public void DataSetLinq40() {

            var numbers = testDS.Tables["Numbers"].AsEnumerable();

            var numberGroups =
                from n in numbers
                group n by n.Field<int>("number") % 5 into g
                select new {Remainder = g.Key, Numbers = g};
    
            foreach (var g in numberGroups) {
                Console.WriteLine("Numbers with a remainder of {0} when divided by 5:", g.Remainder);
                foreach (var n in g.Numbers) {
                    Console.WriteLine(n.Field<int>("number"));
                }
            }       
        }

        [Category("Операторы группировки")]
        [Title("GroupBy - простой 2")]
        [Description("Образец использования Group By для разбиения списка слов по " +
                     "их первой букве.")]
        public void DataSetLinq41() {

            var words4 = testDS.Tables["Words4"].AsEnumerable();

            var wordGroups =
                from w in words4
                group w by w.Field<string>("word")[0] into g
                select new {FirstLetter = g.Key, Words = g};
    
            foreach (var g in wordGroups) {
                Console.WriteLine("Words that start with the letter '{0}':", g.FirstLetter);
                foreach (var w in g.Words) {
                    Console.WriteLine(w.Field<string>("word"));
                }
            }
        }

        [Category("Операторы группировки")]
        [Title("GroupBy - простой 3")]
        [Description("Образец использования Group By для разбиения списка продуктов по категориям.")]
        public void DataSetLinq42() {

            var products = testDS.Tables["Products"].AsEnumerable();

            var productGroups =
                from p in products
                group p by p.Field<string>("Category") into g
                select new {Category = g.Key, Products = g};
                    
            foreach (var g in productGroups) {
                Console.WriteLine("Category: {0}", g.Category);
                foreach (var w in g.Products) {
                    Console.WriteLine("\t" +  w.Field<string>("ProductName"));
                }
            }
        }

        [Category("Операторы группировки")]
        [Title("GroupBy - вложенные")]
        [Description("Образец использования Group By для разбиения списка заказов всех клиентов " +
                     "сначала по годам, а затем по месяцам.")]
        public void DataSetLinq43() {

            var customers = testDS.Tables["Customers"].AsEnumerable();

            var customerOrderGroups = 
                from c in customers
                select
                    new {CompanyName = c.Field<string>("CompanyName"), 
                    YearGroups =
                        from o in c.GetChildRows("CustomersOrders")
                        group o by o.Field<DateTime>("OrderDate").Year into yg
                        select
                            new {Year = yg.Key,
                                MonthGroups = 
                                    from o in yg
                                    group o by o.Field<DateTime>("OrderDate").Month into mg
                                    select new {Month = mg.Key, Orders = mg}
                                }
                };

            foreach(var cog in customerOrderGroups) {
                Console.WriteLine("CompanyName= {0}", cog.CompanyName);
                foreach(var yg in cog.YearGroups) {
                    Console.WriteLine("\t Year= {0}", yg.Year);
                    foreach(var mg in yg.MonthGroups) {
                        Console.WriteLine("\t\t Month= {0}", mg.Month);
                        foreach(var order in mg.Orders) {
                            Console.WriteLine("\t\t\t OrderID= {0} ", order.Field<int>("OrderID"));
                            Console.WriteLine("\t\t\t OrderDate= {0} ", order.Field<DateTime>("OrderDate"));
                        }
                    }
                }
            }           
        }

        private class AnagramEqualityComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y) {
                return getCanonicalString(x) == getCanonicalString(y);
            }

            public int GetHashCode(string obj) {
                return getCanonicalString(obj).GetHashCode();
            }
    
            private string getCanonicalString(string word) {
                char[] wordChars = word.ToCharArray();
                Array.Sort<char>(wordChars);
                return new string(wordChars);
            }
        }

        [Category("Операторы группировки")]
        [Title("GroupBy - сравнение")]
        [Description("Образец использования GroupBy для разбиения обрезанных элементов массива с помощью " +
                     "пользовательского средства, сопоставляющего слова, которые являются анаграммами друг друга.")]
        [LinkedClass("AnagramEqualityComparer")]
        public void DataSetLinq44() {

            var anagrams = testDS.Tables["Anagrams"].AsEnumerable();

            var orderGroups = anagrams.GroupBy(w => w.Field<string>("anagram").Trim(), new AnagramEqualityComparer());

            foreach (var g in orderGroups) {
                Console.WriteLine("Key: {0}", g.Key);
                foreach (var w in g) {
                    Console.WriteLine("\t" +  w.Field<string>("anagram"));
                }
            }
        }

        [Category("Операторы группировки")]
        [Title("GroupBy - сравнение, сопоставление")]
        [Description("Образец использования GroupBy для разбиения обрезанных элементов массива с помощью " +
                     "пользовательского средства, сопоставляющего слова, которые являются анаграммами друг друга, " +
                     "а затем преобразующего результаты в верхний регистр.")]
        [LinkedClass("AnagramEqualityComparer")]
        public void DataSetLinq45() {

            var anagrams = testDS.Tables["Anagrams"].AsEnumerable();

            var orderGroups = anagrams.GroupBy(
                w => w.Field<string>("anagram").Trim(), 
                a => a.Field<string>("anagram").ToUpper(),
                new AnagramEqualityComparer()
                );
                    
            foreach (var g in orderGroups) {
                Console.WriteLine("Key: {0}", g.Key);
                foreach (var w in g) {
                    Console.WriteLine("\t" +  w);
                }
            }         
        }

#endregion

#region "Операторы наборов"

        [Category("Операторы наборов")]
        [Title("Distinct - 1")]
        [Description("Образец использования Distinct для удаления повторяющихся элементов в последовательности " +
                    "множителей 300.")]
        public void DataSetLinq46() {

            var factorsOf300 = testDS.Tables["FactorsOf300"].AsEnumerable();

            var uniqueFactors = factorsOf300.Distinct(DataRowComparer.Default);

            Console.WriteLine("Prime factors of 300:");
            foreach (var f in uniqueFactors) {
                Console.WriteLine(f.Field<int>("factor"));
            }       
        }

        [Category("Операторы наборов")]
        [Title("Distinct - 2")]
        [Description("Пример использования Distinct для поиска с однократным представлением сотрудников")]
        public void DataSetLinq47() {

            var employees1 = testDS.Tables["employees1"].AsEnumerable();

            var employees = employees1.Distinct(DataRowComparer.Default);

            foreach (var row in employees) {
                Console.WriteLine("Id: {0} LastName: {1} Level: {2}", row[0], row[1], row[2]);
            }       
        }

        [Category("Операторы наборов")]
        [Title("Union - 1")]
        [Description("Образец использования Union для создания последовательности, содержащей уникальные значения " +
                     "из обоих массивов.")]
        public void DataSetLinq48() {

            var numbersA = testDS.Tables["NumbersA"].AsEnumerable();
            var numbersB = testDS.Tables["NumbersB"].AsEnumerable();

            var uniqueNumbers = numbersA.Union(numbersB.AsEnumerable(), DataRowComparer.Default);
    
            Console.WriteLine("Unique numbers from both arrays:");
            foreach (var n in uniqueNumbers) {
                Console.WriteLine(n.Field<int>("number"));
            }
       
        }

        [Category("Операторы наборов")]
        [Title("Union - 2")]
        [Description("Пример использования Union для создания одной последовательности на базе двух DataTables.")]
        public void DataSetLinq49() {

            var employees1 = testDS.Tables["employees1"].AsEnumerable();
            var employees2 = testDS.Tables["employees2"].AsEnumerable();

            var employees = employees1.Union(employees2, DataRowComparer.Default);
    
            Console.WriteLine("Union of employees tables");
            foreach (var row in employees) {
                Console.WriteLine("Id: {0} LastName: {1} Level: {2}", row[0], row[1], row[2]);
            }           
        }

        [Category("Операторы наборов")]
        [Title("Intersect - 1")]
        [Description("Образец использования Intersect для создания одной последовательности, содержащей общие значения, " +
                    "используемые совместно обоими массивами.")]
        public void DataSetLinq50() {

            var numbersA = testDS.Tables["NumbersA"].AsEnumerable();
            var numbersB = testDS.Tables["NumbersB"].AsEnumerable();

            var commonNumbers = numbersA.Intersect(numbersB, DataRowComparer.Default);
    
            Console.WriteLine("Common numbers shared by both arrays:");
            foreach (var n in commonNumbers) {
                Console.WriteLine(n.Field<int>(0));
            }  
        }

        [Category("Операторы наборов")]
        [Title("Intersect - 2")]
        [Description("Пример использования Intersect для создания одной последовательности на базе двух DataTables.")]
        public void DataSetLinq51() {

            var employees1 = testDS.Tables["employees1"].AsEnumerable();
            var employees2 = testDS.Tables["employees2"].AsEnumerable();

            var employees = employees1.Intersect(employees2, DataRowComparer.Default);
    
            Console.WriteLine("Intersect of employees tables");
            foreach (var row in employees) {
                Console.WriteLine("Id: {0} LastName: {1} Level: {2}", row[0], row[1], row[2]);
            }   
        }

        [Category("Операторы наборов")]
        [Title("Except - 1")]
        [Description("Образец использования Except для создания последовательности, содержащей значения из таблицы numbersA," +
                     "которых нет также в numbersB.")]
        public void DataSetLinq52() {

            var numbersA = testDS.Tables["NumbersA"].AsEnumerable();
            var numbersB = testDS.Tables["NumbersB"].AsEnumerable();

            var aOnlyNumbers = numbersA.Except(numbersB, DataRowComparer.Default);
    
            Console.WriteLine("Numbers in first array but not second array:");
            foreach (var n in aOnlyNumbers) {
                Console.WriteLine(n["number"]);
            }       
        }

        [Category("Операторы наборов")]
        [Title("Except - 2")]
        [Description("Пример использования Except для создания одной последовательности на базе двух DataTables.")]
        public void DataSetLinq53() {

            var employees1 = testDS.Tables["employees1"].AsEnumerable();
            var employees2 = testDS.Tables["employees2"].AsEnumerable();

            var employees = employees1.Except(employees2, DataRowComparer.Default);
    
            Console.WriteLine("Except of employees tables");
            foreach (var row in employees) {
                Console.WriteLine("Id: {0} LastName: {1} Level: {2}", row[0], row[1], row[2]);
            }   
        }


#endregion

#region "Операторы преобразования"

        [Category("Операторы преобразования")]
        [Title("ToArray")]
        [Description("Образец использования ToArray для немедленного преобразования последовательности в массив.")]
        public void DataSetLinq54() {

            var doubles = testDS.Tables["Doubles"].AsEnumerable();

            var doublesArray = doubles.ToArray();

            var sortedDoubles =
                from d in doublesArray
                orderby d.Field<double>("double") descending
                select d;
    
            Console.WriteLine("Every double from highest to lowest:");
            foreach (var d in sortedDoubles) {
                Console.WriteLine(d.Field<double>("double"));
            }       
        }

        [Category("Операторы преобразования")]
        [Title("ToList")]
        [Description("Пример использования ToList для немедленной вставки последовательности в List<T>.")]
        public void DataSetLinq55() {

            var words = testDS.Tables["Words"].AsEnumerable();

            var wordList = words.ToList(); 
            
            var sortedWords =
                from w in wordList
                orderby w.Field<string>("word")
                select w;
    
            Console.WriteLine("The sorted word list:");
            foreach (var w in sortedWords) {
                Console.WriteLine(w.Field<string>("word").ToLower());
            }        
        }

        [Category("Операторы преобразования")]
        [Title("ToDictionary")]
        [Description("Образец использования ToDictionary для немедленного преобразования последовательности и " +
                    "связанного ключевого выражения в словарь.")]
        public void DataSetLinq56() {
    
            var scoreRecords = testDS.Tables["ScoreRecords"].AsEnumerable();

            var scoreRecordsDict = scoreRecords.ToDictionary(sr => sr.Field<string>("Name"));   
            Console.WriteLine("Bob's score: {0}", scoreRecordsDict["Bob"]["Score"]);          
        }

        [Category("Операторы преобразования")]
        [Title("OfType")]
        [Description("Пример использования OfType для возврата IEnumerable<DataRow>")]
        public void DataSetLinq57() {

            DataTable numbers = testDS.Tables["Numbers"];

            var rows = numbers.Rows.OfType<DataRow>();
            foreach (DataRow d in rows) {
                Console.WriteLine(d.Field<int>("number"));
            }     
        }

#endregion

#region "Операторы элементов"

        [Category("Операторы элементов")]
        [Title("First - простой")]
        [Description("Образец использования First для возврата первого подходящего элемента " +
                     "как\"Товар\", вместо того чтобы возвращать последовательность, содержащую \"Товар\".")]
        public void DataSetLinq58() {

            var products = testDS.Tables["Products"].AsEnumerable();

            DataRow product12 = (
                from p in products
                where (int)p["ProductID"] == 12
                select p )
                .First();

            Console.WriteLine("ProductId: " + product12.Field<int>("ProductId"));
            Console.WriteLine("ProductName: " + product12.Field<string>("ProductName"));        
        }

        [Category("Операторы элементов")]
        [Title("First - условный")]
        [Description("Образец использования First для поиска первого элемента массива, начинающегося с \"o\".")]
        public void DataSetLinq59() {

            var digits = testDS.Tables["Digits"].AsEnumerable();

            var startsWithO = digits.First(s => s.Field<string>("digit")[0] == 'o');
        
            Console.WriteLine("A string starting with 'o': {0}", startsWithO.Field<string>("digit"));          
        }

        [Category("Операторы элементов")]
        [Title("ElementAt")]
        [Description("Образец использования ElementAt для получения второго числа, превышающего 5, " +
                     "из массива.")]
        public void DataSetLinq64() {

            var numbers = testDS.Tables["Numbers"].AsEnumerable();

            var fourthLowNum = (
                from n in numbers
                where n.Field<int>("number") > 5
                select n.Field<int>("number"))
            .ElementAt(3);  // 3, поскольку последовательности используют индексацию на базе 0

            Console.WriteLine("Second number > 5: {0}", fourthLowNum);
        }

        [Category("Операторы элементов")]
        [Title("По умолчанию, если пустой")]
        [Description("Пример использования оператора DefaultIfEmpty для возврата специфического для пользователя значения по умолчанию при пустом содержимом исходной последовательности.")]
        public void DataSetLinq105() {
        
            //создать пустую таблицу
            var emptyTable = new DataTable("EmptyTable").AsEnumerable();
            DataTable numbers = testDS.Tables["Numbers"];

            //создать строку по умолчанию
            DataRow defaultRow = numbers.NewRow();
            defaultRow["number"] = 101;

            var defaultRowSequence = emptyTable.DefaultIfEmpty(defaultRow);

            foreach(DataRow row in defaultRowSequence) {
                Console.WriteLine(row.Field<int>("number"));
            }    
        }

#endregion

#region "Операторы создания"

        [Category("Операторы создания")]
        [Title("Range")]
        [Description("Образец использования Range для создания последовательности чисел от 100 до 149, " +
                     "которая используется для определения, какие числа в этом диапазоне являются четными, а какие нечетными.")]
        public void DataSetLinq65() {

            var numbers =
                from n in Enumerable.Range(100, 50)
                select new {Number = n, OddEven = n % 2 == 1 ? "odd" : "even"};
    
            foreach (var n in numbers) {
                Console.WriteLine("The number {0} is {1}.", n.Number, n.OddEven);
            }   
        }

        [Category("Операторы создания")]
        [Title("Repeat")]
        [Description("Образец использования Repeat для создания последовательности, в которой десять раз встречается число 7.")]
        public void DataSetLinq66() {

            var numbers = Enumerable.Repeat(7, 10);
    
            foreach (var n in numbers) {
                Console.WriteLine(n);
            }           
        }

#endregion

#region "Кванторы"

        [Category("Кванторы")]
        [Title("Any - простой")]
        [Description("Образец использования Any для определения наличия в массиве слов, которые " +
                     "содержат подстроку \"ei\".")]
        public void DataSetLinq67() {
    
            var words2 = testDS.Tables["Words2"].AsEnumerable();

            bool iAfterE = words2.Any(w => w.Field<string>("word").Contains("ei"));

            Console.WriteLine("There is a word that contains in the list that contains 'ei': {0}", iAfterE);          
        }

        [Category("Кванторы")]
        [Title("Any - с группировкой")]
        [Description("Образец использования Any для возврата сгруппированного списка продуктов для категорий, " +
                     "у которых по крайней мере одного продукта нет на складе.")]
        public void DataSetLinq69() {

            var products = testDS.Tables["Products"].AsEnumerable();

            var productGroups =
                from p in products
                group p by p.Field<string>("Category") into g
                where g.Any(p => p.Field<int>("UnitsInStock") == 0)
                select new {Category = g.Key, Products = g};

            foreach(var pg in productGroups) {
                Console.WriteLine(pg.Category);
                foreach(var p in pg.Products) {
                    Console.WriteLine("\t" + p.Field<string>("ProductName"));
                }
            }     
        }

        [Category("Кванторы")]
        [Title("All - простой")]
        [Description("Образец использования All для определения, содержит ли массив " +
                     "только нечетные числа.")]
        public void DataSetLinq70() {

            var numbers = testDS.Tables["Numbers"].AsEnumerable();

            bool onlyOdd = numbers.All(n => n.Field<int>("number") % 2 == 1);

            Console.WriteLine("The list contains only odd numbers: {0}", onlyOdd);         
        }

        [Category("Кванторы")]
        [Title("All - с группировкой")]
        [Description("Образец использования Any для возврата сгруппированного списка продуктов для категорий, " +
                     "у которых все продукты есть в запасе.")]
        public void DataSetLinq72() {

            var products = testDS.Tables["Products"].AsEnumerable();

            var productGroups =
                from p in products
                group p by p.Field<string>("Category") into g
                where g.All(p => p.Field<int>("UnitsInStock") > 0)
                select new {Category = g.Key, Products = g};
            
            foreach(var pg in productGroups) {
                Console.WriteLine(pg.Category);
                foreach(var p in pg.Products) {
                    Console.WriteLine("\t" + p.Field<string>("ProductName"));
                }
            } 
        }
       
        [Category("Кванторы")]
        [Title("Contains")]
        [Description("Пример использования оператора Contains для проверки того, содержит ли исходная последовательность конкретный экземпляр DataRow.")]
        public void DataSetLinq102() {

            var numbers = testDS.Tables["Numbers"];

            //Поиск DataRow с номером == 3
            DataRow rowToFind = null;
            foreach(DataRow r in numbers.Rows) {
                if (r.Field<int>("number") == 3) {
                    rowToFind = r;
                    break;
                }
            }

            var foundRow = numbers.AsEnumerable().Contains(rowToFind);

            Console.WriteLine("Found Row: {0}", foundRow);
            
        }

#endregion

#region "Статистические операторы"

        [Category("Статистические операторы")]
        [Title("Count - простой")]
        [Description("Образец использования Count для вычисления количества уникальных множителей числа 300.")]
        public void DataSetLinq73() {

            var factorsOf300 = testDS.Tables["FactorsOf300"].AsEnumerable();

            int uniqueFactors = factorsOf300.Distinct().Count();
            Console.WriteLine("There are {0} unique factors of 300.", uniqueFactors);            
        }

        [Category("Статистические операторы")]
        [Title("Count - условный")]
        [Description("Образец использования Count для получения количества нечетных целых чисел в массиве.")]
        public void DataSetLinq74() {

            var numbers = testDS.Tables["Numbers"].AsEnumerable();

            int oddNumbers = numbers.Count(n => n.Field<int>("number") % 2 == 1);
            Console.WriteLine("There are {0} odd numbers in the list.", oddNumbers);          
        }

        [Category("Статистические операторы")]
        [Title("Count - вложенный")]
        [Description("Образец использования Count для возврата списка клиентов, а также количества заказов, " +
                     "у каждого.")]
        public void DataSetLinq76() {

            var customers = testDS.Tables["Customers"].AsEnumerable();

            var orderCounts = from c in customers
                where c.Field<string>("CustomerID") != "AAAAA"
                select new {CustomerID = c.Field<string>("CustomerID"), 
                    OrderCount = c.GetChildRows("CustomersOrders").Count()};

            ObjectDumper.Write(orderCounts);         
        }

        [Category("Статистические операторы")]
        [Title("Count - с группировкой")]
        [Description("Образец использования Count для возврата списка категорий, а также количества продуктов, " +
                     "у каждого.")]
        public void DataSetLinq77() {

            var products = testDS.Tables["Products"].AsEnumerable();

            var categoryCounts =
                from p in products
                group p by p.Field<string>("Category") into g
                select new {Category = g.Key, ProductCount = g.Count()};
    
            ObjectDumper.Write(categoryCounts);     
        }

        
        [Category("Статистические операторы")]
        [Title("Простой счетчик Long")]
        [Description("Получить значение счетчика типа Long")]
        public void DataSetLinq103() {

            var products = testDS.Tables["Products"].AsEnumerable();

            long numberOfProducts = products.LongCount();
            Console.WriteLine("There are {0} products", numberOfProducts);
        }

        [Category("Статистические операторы")]
        [Title("Условный счетчик Long")]
        [Description("Образец использования Count для получения с типом Long количества нечетных целых чисел в массиве")]
        public void DataSetLinq104() {

            var numbers = testDS.Tables["Numbers"].AsEnumerable();

            long oddNumbers = numbers.LongCount(n => (int) n["number"] % 2 == 1);    
            Console.WriteLine("There are {0} odd numbers in the list.", oddNumbers);        
        }

        [Category("Статистические операторы")]
        [Title("Сумма - проекция")]
        [Description("Образец использования Sum для получения общего количества букв во всех словах " +
                     "в массиве.")]
        public void DataSetLinq79() {

            var words = testDS.Tables["Words"].AsEnumerable();

            double totalChars = words.Sum(w => w.Field<string>("word").Length);
            Console.WriteLine("There are a total of {0} characters in these words.", totalChars);      
        }

        [Category("Статистические операторы")]
        [Title("Sum - с группировкой")]
        [Description("Образец использования Sum для получения общего количества единиц в запасе по каждой категории продуктов.")]
        public void DataSetLinq80() {

            var products = testDS.Tables["Products"].AsEnumerable();

            var categories =
                from p in products
                group p by p.Field<string>("Category") into g
                select new {Category = g.Key, TotalUnitsInStock = g.Sum(p => p.Field<int>("UnitsInStock"))};

            ObjectDumper.Write(categories);           
        }

        [Category("Статистические операторы")]
        [Title("Min - проекция")]
        [Description("Образец использования Min для вычисления длины самого короткого слова в массиве.")]
        public void DataSetLinq82() {

            var words = testDS.Tables["Words"].AsEnumerable();

            int shortestWord = words.Min(w => w.Field<string>("word").Length);    
            Console.WriteLine("The shortest word is {0} characters long.", shortestWord);
        }

        [Category("Статистические операторы")]
        [Title("Min - с группировкой")]
        [Description("Образец использования Min для поиска наименьшей цены продуктов в каждой категории.")]
        public void DataSetLinq83() {

            var products = testDS.Tables["Products"].AsEnumerable();

            var categories =
                from p in products
                group p by p.Field<string>("Category") into g
                select new {Category = g.Key, CheapestPrice = g.Min(p => p.Field<decimal>("UnitPrice"))};

            ObjectDumper.Write(categories);
        }

        [Category("Статистические операторы")]
        [Title("Min - элементы")]
        [Description("Образец использования Min для получения продуктов с наименьшей ценой в каждой категории.")]
        public void DataSetLinq84() {

            var products = testDS.Tables["Products"].AsEnumerable();

            var categories =
                from p in products
                group p by p.Field<string>("Category") into g
                let minPrice = g.Min(p => p.Field<decimal>("UnitPrice"))
                select new {Category = g.Key, CheapestProducts = g.Where(p => p.Field<decimal>("UnitPrice") == minPrice)};

            foreach (var g in categories) {
                Console.WriteLine("Category: {0}", g.Category);
                Console.WriteLine("CheapestProducts:");
                foreach (var w in g.CheapestProducts) {
                    Console.WriteLine("\t" +  w.Field<string>("ProductName"));
                }
            }            
        }

        [Category("Статистические операторы")]
        [Title("Max - проекция")]
        [Description("Образец использования Max для получения длины наибольшего слова в массиве.")]
        public void DataSetLinq86() {

            var words = testDS.Tables["Words"].AsEnumerable();

            int longestLength = words.Max(w => w.Field<string>("word").Length);
    
            Console.WriteLine("The longest word is {0} characters long.", longestLength);       
        }

        [Category("Статистические операторы")]
        [Title("Max - с группировкой")]
        [Description("Образец использования Max для получения наибольшей цены в каждой категории продуктов.")]
        public void DataSetLinq87() {

            var products = testDS.Tables["Products"].AsEnumerable();

            var categories =
                from p in products
                group p by p.Field<string>("Category") into g
                select new {Category = g.Key, MostExpensivePrice = g.Max(p => p.Field<decimal>("UnitPrice"))};

            ObjectDumper.Write(categories);
        }

        [Category("Статистические операторы")]
        [Title("Max - элементы")]
        [Description("Образец использования Max для получения продуктов с наибольшей ценой в каждой категории.")]
        public void DataSetLinq88() {

            var products = testDS.Tables["Products"].AsEnumerable();

            var categories =
                from p in products
                group p by p.Field<string>("Category") into g
                let maxPrice = g.Max(p => p.Field<decimal>("UnitPrice"))
                select new {Category = g.Key, MostExpensiveProducts = g.Where(p => p.Field<decimal>("UnitPrice")== maxPrice)};

            foreach (var g in categories) {
                Console.WriteLine("Category: {0}", g.Category);
                Console.WriteLine("MostExpensiveProducts:");
                foreach (var w in g.MostExpensiveProducts) {
                    Console.WriteLine("\t" +  w.Field<string>("ProductName"));
                }
            }
        }

        [Category("Статистические операторы")]
        [Title("Average - проекция")]
        [Description("Образец использования Average для вычисления средней длины слов в массиве.")]
        public void DataSetLinq90() {

            var words = testDS.Tables["Words"].AsEnumerable();

            double averageLength = words.Average(w => w.Field<string>("word").Length);  
            Console.WriteLine("The average word length is {0} characters.", averageLength);
        }

        [Category("Статистические операторы")]
        [Title("Average - с группировкой")]
        [Description("Образец использования Average для вычисления средней цены продуктов в каждой категории.")]
        public void DataSetLinq91() {

            var products = testDS.Tables["Products"].AsEnumerable();

            var categories =
                from p in products
                group p by p.Field<string>("Category") into g
                select new {Category = g.Key, AveragePrice = g.Average(p => p.Field<decimal>("UnitPrice"))};

            ObjectDumper.Write(categories);            
        }

        [Category("Статистические операторы")]
        [Title("Aggregate - начальное значение")]
        [Description("Образец использования Aggregate для создания текущего сальдо счета, которое " +
                     "отражает все вычеты с исходного сальдо, равного 100, пока " +
                     "сальдо не опустится ниже 0.")]
        public void DataSetLinq93() {

            var attemptedWithdrawals = testDS.Tables["AttemptedWithdrawals"].AsEnumerable();
            
            double startBalance = 100.0;
    
            var endBalance = 
                attemptedWithdrawals.Aggregate(startBalance,
                (balance, nextWithdrawal) =>
                nextWithdrawal.Field<int>("withdrawal") <= balance ? 
                    balance - nextWithdrawal.Field<int>("withdrawal") : balance);
    
            Console.WriteLine("Ending balance: {0}", endBalance);         
        }

#endregion

#region "Прочие операторы"

        [Category("Прочие операторы")]
        [Title("Concat – 1")]
        [Description("Образец использования Concat для создания последовательности, содержащей все значения " +
                     "массива, одно за другим.")]
        public void DataSetLinq94() {

            var numbersA = testDS.Tables["NumbersA"].AsEnumerable();
            var numbersB = testDS.Tables["NumbersB"].AsEnumerable();

            var allNumbers = numbersA.Concat(numbersB);
    
            Console.WriteLine("All numbers from both arrays:");
            foreach (var n in allNumbers) {
                Console.WriteLine(n.Field<int>("number"));
            }            
        }

        [Category("Прочие операторы")]
        [Title("Concat - 2")]
        [Description("Образец использования Concat для создания последовательности, содержащей имена " +
                     "всех заказчиков и товаров, в том числе все дубликаты.")]
        public void DataSetLinq95() {

            var products = testDS.Tables["Products"].AsEnumerable();
            var customers = testDS.Tables["Customers"].AsEnumerable();

            var customerNames = customers.Select(r => r.Field<string>("CompanyName"));
            var productNames = products.Select(r => r.Field<string>("ProductName"));
    
            var allNames = customerNames.Concat(productNames);
    
            Console.WriteLine("Customer and product names:");
            foreach (var n in allNames) {
                Console.WriteLine(n);
            }     
        }

        [Category("Прочие операторы")]
        [Title("SequenceEqual - 1")]
        [Description("Пример использования SequenceEqual для проверки того, все ли элементы двух последовательностей соответствуют друг другу " +
                     "в одном и том же порядке.")]
        public void DataSetLinq96() {

            var words = testDS.Tables["Words"].AsEnumerable();

            bool match = words.SequenceEqual(words);    
            Console.WriteLine("The sequences match: {0}", match);
        }

        [Category("Прочие операторы")]
        [Title("SequenceEqual - 2")]
        [Description("Пример использования SequenceEqual для проверки того, все ли элементы двух последовательностей соответствуют друг другу " +
                     "в различном порядке.")]
        public void DataSetLinq97() {

            var words = testDS.Tables["Words"].AsEnumerable();
            var words2 = testDS.Tables["Words2"].AsEnumerable();

            bool match = words.SequenceEqual(words2); 
            Console.WriteLine("The sequences match: {0}", match);
        }



        [Category("Пользовательские операторы последовательности")]
        [Title("Combine")]
        [Description("Образец использования пользовательского оператора последовательности Combine, чтобы рассчитать " +
                     "скалярное произведение двух векторов.")]
        [LinkedClass("CustomSequenceOperators")]
        public void DataSetLinq98() {

            var numbersA = testDS.Tables["NumbersA"].AsEnumerable();
            var numbersB = testDS.Tables["NumbersB"].AsEnumerable();

            int dotProduct = numbersA.Combine(numbersB, (a, b) => a.Field<int>("number") * b.Field<int>("number")).Sum();
            Console.WriteLine("Dot product: {0}", dotProduct);
        }

        [Category("Выполнение запроса")]
        [Title("ToLookup - селектор элементов")]
        [Description("Образец немедленного выполнения запросов с операторами, " +
                     "например ToLookup().")]
        public void DataSetLinq108() {
    
            var products = testDS.Tables["Products"].AsEnumerable();

            var productsLookup = products.ToLookup(p => p.Field<string>("Category"));
    
            IEnumerable<DataRow> confections = productsLookup["Confections"];

            Console.WriteLine("Number of categories: {0}", productsLookup.Count);
            foreach(var product in confections) {
                Console.WriteLine("ProductName: {0}", product.Field<string>("ProductName"));
            }    
        }

        [Category("Выполнение запроса")]
        [Title("Повторное использование запроса")]
        [Description("Образец повторного использования запросов с отложенным выполнением " +
                     "после изменения данных и последующей работы с новыми данными.")]
        public void DataSetLinq101() {

            var numbers = testDS.Tables["Numbers"];

            var lowNumbers =
                from n in numbers.AsEnumerable()
                where n.Field<int>("number") <= 3
                select n;

            Console.WriteLine("First run numbers <= 3:");
            foreach (var n in lowNumbers) {
                Console.WriteLine(n.Field<int>("number"));
            }

            for (int i = 0; i < 10; i++) {
                numbers.Rows[i]["number"] = -(int)numbers.Rows[i]["number"];
            }

            // Во время второго прохода тот же самый объект запроса,
            // lowNumbers, будет выполнять итерацию с новым состоянием
            // numbers[], генерируя другие результаты:
            Console.WriteLine("Second run numbers <= 3:");
            foreach (var n in lowNumbers) {
                Console.WriteLine(n.Field<int>("number"));
            }

            //очистка таблицы чисел
            for (int i = 0; i < 10; i++) {
                numbers.Rows[i]["number"] = -(int)numbers.Rows[i]["number"];
            }           
        }
#endregion

#region "Примеры загрузки набора данных"

        [Category("Примеры загрузки набора данных")]
        [Title("Загрузка результатов запроса в DataTable")]
        [Description("Создание и загрузка DataTable данными последовательности")]
        public void DataSetLinq109() {

            var customers = testDS.Tables["Customers"].AsEnumerable();
            var orders = testDS.Tables["Orders"].AsEnumerable();

            var smallOrders =
                from c in customers
                from o in orders
                where c.Field<string>("CustomerID") == o.Field<string>("CustomerID") &&
                    o.Field<decimal>("Total") < 500.00M
                select new {CustomerID = (string) c["CustomerID"], OrderID = (int) o["OrderID"], Total = (decimal) o["Total"]};

            DataTable myOrders = new DataTable();
            myOrders.Columns.Add("CustomerID", typeof(string));
            myOrders.Columns.Add("OrderID", typeof(int));
            myOrders.Columns.Add("Total", typeof(decimal));

            foreach (var result in smallOrders.Take(10))
            {
                myOrders.Rows.Add(new object[] { result.CustomerID, result.OrderID, result.Total });
            }

            PrettyPrintDataTable(myOrders);
        }

        [Category("Примеры загрузки набора данных")]
        [Title("Использование таблицы CopyToData")]
        [Description("Загрузить результаты запроса в существующую таблицу")]
        public void DataSetLinq117a()
        {

            var customers = testDS.Tables["Customers"].AsEnumerable();
            var orders = testDS.Tables["Orders"].AsEnumerable();

            var query = from o in orders
                        where o.Field<decimal>("Total") < 500.00M
                        select o;


            DataTable results = query.CopyToDataTable();

            PrettyPrintDataTable(results);
        }


        private void PrettyPrintDataReader(DbDataReader reader) {
            while(reader.Read()) {
                StringBuilder sb = new StringBuilder();
                for(int ii = 0; ii< reader.FieldCount; ii++) {
                    sb.AppendFormat("{0} = {1}  ", reader.GetName(ii), reader.IsDBNull(ii) ? "null" : reader[ii]);
                }           
                Console.WriteLine(sb.ToString());
            }
        }

        private void PrettyPrintDataTable(DataTable table) {
            Console.WriteLine("Table: {0}", table.TableName);
            foreach(DataRow row in table.Rows) {
                StringBuilder sb = new StringBuilder();
                foreach(DataColumn dc in table.Columns) {
                    sb.AppendFormat("{0} = {1}  ", dc.ColumnName, row.IsNull(dc) ? "null" : row[dc]);
                }
                Console.WriteLine(sb.ToString());
            }
        }
#endregion

#region "Linq через TypedDataSet"

        [Category("LINQ через TypedDataSet")]
        [Title("TypedDataSet - простой")]
        [Description("Простые запросы с использованием типизированного набора данных")]
        public void DataSetLinq115() {

            
            EmployeeDataSet.EmployeesTableDataTable employees = new EmployeeDataSet.EmployeesTableDataTable();
            employees.AddEmployeesTableRow(5, "Jeff Jones", 60000);
            employees.AddEmployeesTableRow(6, "Geoff Webber", 85000);
            employees.AddEmployeesTableRow(7, "Alan Fox", 85000);
            employees.AddEmployeesTableRow(8, "Dwight Schute", 101000);
            employees.AddEmployeesTableRow(9, "Chaz Hoover", 99999);

            var q = employees.Where(e => e.Salary >= 85000).OrderBy(e => e.Name);

            foreach(var emp in q) {
                Console.WriteLine("Id = {0}, Name = {1}", emp.ID, emp.Name);
            }
        }

        [Category("LINQ через TypedDataSet")]
        [Title("TypedDataSet - проекция ")]
        [Description("Проекция, использующая типизированный DataSet")]
        public void DataSetLinq116() {

            EmployeeDataSet.EmployeesTableDataTable employees = new EmployeeDataSet.EmployeesTableDataTable();
            employees.AddEmployeesTableRow(5, "Jeff Jones", 60000);
            employees.AddEmployeesTableRow(6, "Geoff Webber", 85000);
            employees.AddEmployeesTableRow(7, "Alan Fox", 85000);
            employees.AddEmployeesTableRow(8, "Dwight Schute", 101000);
            employees.AddEmployeesTableRow(9, "Chaz Hoover", 99999);

            var q = employees.Select(emp => new {EmployeeID = emp.ID, EmployeeName = emp.Name, Employee = emp}).OrderBy(e => e.EmployeeName);

            foreach(var o in q) {
                Console.WriteLine("Id = {0}, Name = {1}", o.EmployeeID, o.EmployeeName);
            }
        }

        [Category("LINQ через TypedDataSet")]
        [Title("TypedDataSet - загрузка таблицы данных ")]
        [Description("Загрузить результаты запроса в существующую таблицу")]
        public void DataSetLinq117() {

            EmployeeDataSet.EmployeesTableDataTable employees = new EmployeeDataSet.EmployeesTableDataTable();
            employees.AddEmployeesTableRow(5, "Jeff Jones", 60000);
            employees.AddEmployeesTableRow(6, "Geoff Webber", 85000);
            employees.AddEmployeesTableRow(7, "Alan Fox", 85000);
            employees.AddEmployeesTableRow(8, "Dwight Schute", 101000);
            employees.AddEmployeesTableRow(9, "Chaz Hoover", 99999);
            
            EmployeeDataSet.EmployeesTableDataTable filteredEmployees = new EmployeeDataSet.EmployeesTableDataTable();

            var q = from e in employees
                    where e.ID > 7
                    select e;
            
            q.CopyToDataTable(filteredEmployees, LoadOption.OverwriteChanges);
            
            PrettyPrintDataTable(filteredEmployees);
        }

#endregion

#region "Операторы соединения"

        [Category("Операторы соединения")]
        [Title("Join - простой")]
        [Description("Простое присоединение через две таблицы")]
        public void DataSetLinq118() {
            var customers = testDS.Tables["Customers"].AsEnumerable();
            var orders = testDS.Tables["Orders"].AsEnumerable();

            var smallOrders = customers.Join(orders, o => o.Field<string>("CustomerID"), c => c.Field<string>("CustomerID"), 
              (c, o) =>  new {CustomerID = c.Field<string>("CustomerID"), OrderID = o.Field<int>("OrderID"), 
                    Total = o.Field<decimal>("Total")});
              
            ObjectDumper.Write(smallOrders);
        }

        [Category("Операторы соединения")]
        [Title("Соединение с группировкой результатов")]
        [Description("Соединение с группировкой результатов")]
        public void DataSetLinq119() {

            var customers = testDS.Tables["Customers"].AsEnumerable();
            var orders = testDS.Tables["Orders"].AsEnumerable();

            var groupedOrders = customers.Join(orders, o => o.Field<string>("CustomerID"), c => c.Field<string>("CustomerID"), 
              (c, o) =>  new {CustomerID = c.Field<string>("CustomerID"), OrderID = o.Field<int>("OrderID"), 
                    Total = o.Field<decimal>("Total")})
                    .GroupBy(r => r.OrderID);

            foreach(var group in groupedOrders) {
                foreach(var order in group) {
                    ObjectDumper.Write(order);
                }
            }
        }

        [Category("Операторы соединения")]
        [Title("Group Join")]
        [Description("Простое групповое соединение")]
        public void DataSetLinq120() {
            var customers = testDS.Tables["Customers"].AsEnumerable();
            var orders = testDS.Tables["Orders"].AsEnumerable();

            var q = from c in customers
                    join o in orders on c.Field<string>("CustomerID") equals o.Field<string>("CustomerID") into ords
                    select new {CustomerID = c.Field<string>("CustomerID"), ords=ords.Count()};   

            foreach(var r in q) {
                Console.WriteLine("CustomerID: {0}  Orders Count: {1}", r.CustomerID, r.ords);
            }
        }

#endregion
    }

#region "Вспомогательная функция тестирования"
    internal class TestHelper
    {
        //private readonly static string dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @\"LINQ Preview\Data\");
        // private readonly static string dataPath = @\"F:\Dev\Feb CTP\Data\"; 
        private readonly static string dataPath = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..\Data\"));

        internal static DataSet CreateTestDataset() {
            DataSet ds = new DataSet();
            
            // Таблица заказчиков
            ds.Tables.Add(CreateNumbersTable());
            ds.Tables.Add(CreateLowNumbersTable());
            ds.Tables.Add(CreateEmptyNumbersTable());
            ds.Tables.Add(CreateProductList());
            ds.Tables.Add(CreateDigitsTable());
            ds.Tables.Add(CreateWordsTable());
            ds.Tables.Add(CreateWords2Table());
            ds.Tables.Add(CreateWords3Table());
            ds.Tables.Add(CreateWords4Table());
            ds.Tables.Add(CreateAnagramsTable());
            ds.Tables.Add(CreateNumbersATable());
            ds.Tables.Add(CreateNumbersBTable());
            ds.Tables.Add(CreateFactorsOf300());
            ds.Tables.Add(CreateDoublesTable());
            ds.Tables.Add(CreateScoreRecordsTable());
            ds.Tables.Add(CreateAttemptedWithdrawalsTable());
            ds.Tables.Add(CreateEmployees1Table());
            ds.Tables.Add(CreateEmployees2Table());

            CreateCustomersAndOrdersTables(ds);

            ds.AcceptChanges();
            return ds;
        }


        private static DataTable CreateNumbersTable() {

            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            DataTable table = new DataTable("Numbers");
            table.Columns.Add("number", typeof(int));

            foreach (int n in numbers) {
                table.Rows.Add(new object[] {n});
            }

            return table;
        }

        private static DataTable CreateEmptyNumbersTable() {

            DataTable table = new DataTable("EmptyNumbers");
            table.Columns.Add("number", typeof(int));
            return table;
        }

        private static DataTable CreateDigitsTable() {

            string[] digits = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
            DataTable table = new DataTable("Digits");
            table.Columns.Add("digit", typeof(string));

            foreach (string digit in digits) {
                table.Rows.Add(new object[] {digit});
            }
            return table;
        }

        private static DataTable CreateWordsTable() {
            string[] words = { "aPPLE", "BlUeBeRrY", "cHeRry" };
            DataTable table = new DataTable("Words");
            table.Columns.Add("word", typeof(string));

            foreach (string word in words) {
                table.Rows.Add(new object[] {word});
            }
            return table;
        }

        private static DataTable CreateWords2Table() {
            string[] words = { "believe", "relief", "receipt", "field" };
            DataTable table = new DataTable("Words2");
            table.Columns.Add("word", typeof(string));

            foreach (string word in words) {
                table.Rows.Add(new object[] {word});
            }
            return table;
        }

        private static DataTable CreateWords3Table() {
            string[] words = { "aPPLE", "AbAcUs", "bRaNcH", "BlUeBeRrY", "ClOvEr", "cHeRry"};
            DataTable table = new DataTable("Words3");
            table.Columns.Add("word", typeof(string));

            foreach (string word in words) {
                table.Rows.Add(new object[] {word});
            }
            return table;
        }

        private static DataTable CreateWords4Table() {
            string[] words = { "blueberry", "chimpanzee", "abacus", "banana", "apple", "cheese" };
            DataTable table = new DataTable("Words4");
            table.Columns.Add("word", typeof(string));

            foreach (string word in words) {
                table.Rows.Add(new object[] {word});
            }
            return table;
        }

        private static DataTable CreateAnagramsTable() {
            string[] anagrams = {"from   ", " salt", " earn ", "  last   ", " near ", " form  "};
            DataTable table = new DataTable("Anagrams");
            table.Columns.Add("anagram", typeof(string));

            foreach (string word in anagrams) {
                table.Rows.Add(new object[] {word});
            }
            return table;
        }

        private static DataTable CreateScoreRecordsTable() {
            var scoreRecords = new [] { new {Name = "Alice", Score = 50},
                                new {Name = "Bob"  , Score = 40},
                                new {Name = "Cathy", Score = 45}
                            };

            DataTable table = new DataTable("ScoreRecords");
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Score", typeof(int));

            foreach (var r in scoreRecords) {
                table.Rows.Add(new object[] {r.Name, r.Score});
            }
            return table;
        }

        private static DataTable CreateAttemptedWithdrawalsTable() {
            int[] attemptedWithdrawals = { 20, 10, 40, 50, 10, 70, 30 };

            DataTable table = new DataTable("AttemptedWithdrawals");
            table.Columns.Add("withdrawal", typeof(int));

            foreach (var r in attemptedWithdrawals) {
                table.Rows.Add(new object[] {r});
            }
            return table;
        }

        private static DataTable CreateNumbersATable() {
            int[] numbersA = { 0, 2, 4, 5, 6, 8, 9 };
            DataTable table = new DataTable("NumbersA");
            table.Columns.Add("number", typeof(int));

            foreach (int number in numbersA) {
                table.Rows.Add(new object[] {number});
            }
            return table;
        }

        private static DataTable CreateNumbersBTable() {
            int[] numbersB = { 1, 3, 5, 7, 8 };
            DataTable table = new DataTable("NumbersB");
            table.Columns.Add("number", typeof(int));

            foreach (int number in numbersB) {
                table.Rows.Add(new object[] {number});
            }
            return table;
        }

        private static DataTable CreateLowNumbersTable() {
            int[] lowNumbers = { 1, 11, 3, 19, 41, 65, 19 };
            DataTable table = new DataTable("LowNumbers");
            table.Columns.Add("number", typeof(int));

            foreach (int number in lowNumbers) {
                table.Rows.Add(new object[] {number});
            }
            return table;
        }

        private static DataTable CreateFactorsOf300() {
            int[] factorsOf300 = { 2, 2, 3, 5, 5 };

            DataTable table = new DataTable("FactorsOf300");
            table.Columns.Add("factor", typeof(int));

            foreach (int factor in factorsOf300) {
                table.Rows.Add(new object[] {factor});
            }
            return table;
        }

        private static DataTable CreateDoublesTable() {
            double[] doubles = { 1.7, 2.3, 1.9, 4.1, 2.9 };

            DataTable table = new DataTable("Doubles");
            table.Columns.Add("double", typeof(double));

            foreach (double d in doubles) {
                table.Rows.Add(new object[] {d});
            }
            return table;
        }

        private static DataTable CreateEmployees1Table() {

            DataTable table = new DataTable("Employees1");
            table.Columns.Add("id", typeof(int));
            table.Columns.Add("name", typeof(string));
            table.Columns.Add("worklevel", typeof(int));

            table.Rows.Add(new object[] {1, "Jones", 5});
            table.Rows.Add(new object[] {2, "Smith", 5});
            table.Rows.Add(new object[] {2, "Smith", 5});
            table.Rows.Add(new object[] {3, "Smith", 6});
            table.Rows.Add(new object[] {4, "Arthur", 11});
            table.Rows.Add(new object[] {5, "Arthur", 12});

            return table;
        }

        private static DataTable CreateEmployees2Table() {

            DataTable table = new DataTable("Employees2");
            table.Columns.Add("id", typeof(int));
            table.Columns.Add("lastname", typeof(string));
            table.Columns.Add("level", typeof(int));

            table.Rows.Add(new object[] {1, "Jones", 10});
            table.Rows.Add(new object[] {2, "Jagger", 5});
            table.Rows.Add(new object[] {3, "Thomas", 6});
            table.Rows.Add(new object[] {4, "Collins", 11});
            table.Rows.Add(new object[] {4, "Collins", 12});
            table.Rows.Add(new object[] {5, "Arthur", 12});

            return table;
        }

        private static void CreateCustomersAndOrdersTables(DataSet ds) {
            
            DataTable customers = new DataTable("Customers");
            customers.Columns.Add("CustomerID", typeof(string));
            customers.Columns.Add("CompanyName", typeof(string));
            customers.Columns.Add("Address", typeof(string));
            customers.Columns.Add("City", typeof(string));
            customers.Columns.Add("Region", typeof(string));
            customers.Columns.Add("PostalCode", typeof(string));
            customers.Columns.Add("Country", typeof(string));
            customers.Columns.Add("Phone", typeof(string));
            customers.Columns.Add("Fax", typeof(string));

            ds.Tables.Add(customers);

            DataTable orders = new DataTable("Orders");
            
            orders.Columns.Add("OrderID", typeof(int));
            orders.Columns.Add("CustomerID", typeof(string));
            orders.Columns.Add("OrderDate", typeof(DateTime));
            orders.Columns.Add("Total", typeof(decimal));

            ds.Tables.Add(orders);

            DataRelation co = new DataRelation("CustomersOrders", customers.Columns["CustomerID"],  orders.Columns["CustomerID"], true);
            ds.Relations.Add(co);

            string customerListPath = Path.GetFullPath(Path.Combine(dataPath, "customers.xml"));

            var customerList = (
                from e in XDocument.Load(customerListPath).
                          Root.Elements("customer")
                select new Customer {
                    CustomerID = (string)e.Element("id"),
                    CompanyName = (string)e.Element("name"),
                    Address = (string)e.Element("address"),
                    City = (string)e.Element("city"),
                    Region = (string)e.Element("region"),
                    PostalCode = (string)e.Element("postalcode"),
                    Country = (string)e.Element("country"),
                    Phone = (string)e.Element("phone"),
                    Fax = (string)e.Element("fax"),
                    Orders = (
                        from o in e.Elements("orders").Elements("order")
                        select new Order {
                            OrderID = (int)o.Element("id"),
                            OrderDate = (DateTime)o.Element("orderdate"),
                            Total = (decimal)o.Element("total") } )
                        .ToArray() } 
                ).ToList();

            foreach (Customer cust in customerList) {
                customers.Rows.Add(new object[] {cust.CustomerID, cust.CompanyName, cust.Address, cust.City, cust.Region,
                                                cust.PostalCode, cust.Country, cust.Phone, cust.Fax});
                foreach (Order order in cust.Orders) {
                    orders.Rows.Add(new object[] {order.OrderID, cust.CustomerID, order.OrderDate, order.Total});
                }
            }
        }

        private static DataTable CreateProductList() {

            DataTable table = new DataTable("Products");
            table.Columns.Add("ProductID", typeof(int));
            table.Columns.Add("ProductName", typeof(string));
            table.Columns.Add("Category", typeof(string));
            table.Columns.Add("UnitPrice", typeof(decimal));
            table.Columns.Add("UnitsInStock", typeof(int));

            var productList = new[] {
              new { ProductID = 1, ProductName = "Chai", Category = "Beverages", 
                UnitPrice = 18.0000M, UnitsInStock = 39 },
              new { ProductID = 2, ProductName = "Chang", Category = "Beverages", 
                UnitPrice = 19.0000M, UnitsInStock = 17 },
              new { ProductID = 3, ProductName = "Aniseed Syrup", Category = "Condiments", 
                UnitPrice = 10.0000M, UnitsInStock = 13 },
              new { ProductID = 4, ProductName = "Chef Anton's Cajun Seasoning", Category = "Condiments", 
                UnitPrice = 22.0000M, UnitsInStock = 53 },
              new { ProductID = 5, ProductName = "Chef Anton's Gumbo Mix", Category = "Condiments", 
                UnitPrice = 21.3500M, UnitsInStock = 0 },
              new { ProductID = 6, ProductName = "Grandma's Boysenberry Spread", Category = "Condiments", 
                UnitPrice = 25.0000M, UnitsInStock = 120 },
              new { ProductID = 7, ProductName = "Uncle Bob's Organic Dried Pears", Category = "Produce", 
                UnitPrice = 30.0000M, UnitsInStock = 15 },
              new { ProductID = 8, ProductName = "Northwoods Cranberry Sauce", Category = "Condiments", 
                UnitPrice = 40.0000M, UnitsInStock = 6 },
              new { ProductID = 9, ProductName = "Mishi Kobe Niku", Category = "Meat/Poultry", 
                UnitPrice = 97.0000M, UnitsInStock = 29 },
              new { ProductID = 10, ProductName = "Ikura", Category = "Seafood", 
                UnitPrice = 31.0000M, UnitsInStock = 31 },
              new { ProductID = 11, ProductName = "Queso Cabrales", Category = "Dairy Products", 
                UnitPrice = 21.0000M, UnitsInStock = 22 },
              new { ProductID = 12, ProductName = "Queso Manchego La Pastora", Category = "Dairy Products", 
                UnitPrice = 38.0000M, UnitsInStock = 86 },
              new { ProductID = 13, ProductName = "Konbu", Category = "Seafood", 
                UnitPrice = 6.0000M, UnitsInStock = 24 },
              new { ProductID = 14, ProductName = "Tofu", Category = "Produce", 
                UnitPrice = 23.2500M, UnitsInStock = 35 },
              new { ProductID = 15, ProductName = "Genen Shouyu", Category = "Condiments", 
                UnitPrice = 15.5000M, UnitsInStock = 39 },
              new { ProductID = 16, ProductName = "Pavlova", Category = "Confections", 
                UnitPrice = 17.4500M, UnitsInStock = 29 },
              new { ProductID = 17, ProductName = "Alice Mutton", Category = "Meat/Poultry", 
                UnitPrice = 39.0000M, UnitsInStock = 0 },
              new { ProductID = 18, ProductName = "Carnarvon Tigers", Category = "Seafood", 
                UnitPrice = 62.5000M, UnitsInStock = 42 },
              new { ProductID = 19, ProductName = "Teatime Chocolate Biscuits", Category = "Confections", 
                UnitPrice = 9.2000M, UnitsInStock = 25 },
              new { ProductID = 20, ProductName = "Sir Rodney's Marmalade", Category = "Confections", 
                UnitPrice = 81.0000M, UnitsInStock = 40 },
              new { ProductID = 21, ProductName = "Sir Rodney's Scones", Category = "Confections", 
                UnitPrice = 10.0000M, UnitsInStock = 3 },
              new { ProductID = 22, ProductName = "Gustaf's Knäckebröd", Category = "Grains/Cereals", 
                UnitPrice = 21.0000M, UnitsInStock = 104 },
              new { ProductID = 23, ProductName = "Tunnbröd", Category = "Grains/Cereals", 
                UnitPrice = 9.0000M, UnitsInStock = 61 },
              new { ProductID = 24, ProductName = "Guaraná Fantástica", Category = "Beverages", 
                UnitPrice = 4.5000M, UnitsInStock = 20 },
              new { ProductID = 25, ProductName = "NuNuCa Nuß-Nougat-Creme", Category = "Confections", 
                UnitPrice = 14.0000M, UnitsInStock = 76 },
              new { ProductID = 26, ProductName = "Gumbär Gummibärchen", Category = "Confections", 
                UnitPrice = 31.2300M, UnitsInStock = 15 },
              new { ProductID = 27, ProductName = "Schoggi Schokolade", Category = "Confections", 
                UnitPrice = 43.9000M, UnitsInStock = 49 },
              new { ProductID = 28, ProductName = "Rössle Sauerkraut", Category = "Produce", 
                UnitPrice = 45.6000M, UnitsInStock = 26 },
              new { ProductID = 29, ProductName = "Thüringer Rostbratwurst", Category = "Meat/Poultry", 
                UnitPrice = 123.7900M, UnitsInStock = 0 },
              new { ProductID = 30, ProductName = "Nord-Ost Matjeshering", Category = "Seafood", 
                UnitPrice = 25.8900M, UnitsInStock = 10 },
              new { ProductID = 31, ProductName = "Gorgonzola Telino", Category = "Dairy Products", 
                UnitPrice = 12.5000M, UnitsInStock = 0 },
              new { ProductID = 32, ProductName = "Mascarpone Fabioli", Category = "Dairy Products", 
                UnitPrice = 32.0000M, UnitsInStock = 9 },
              new { ProductID = 33, ProductName = "Geitost", Category = "Dairy Products", 
                UnitPrice = 2.5000M, UnitsInStock = 112 },
              new { ProductID = 34, ProductName = "Sasquatch Ale", Category = "Beverages", 
                UnitPrice = 14.0000M, UnitsInStock = 111 },
              new { ProductID = 35, ProductName = "Steeleye Stout", Category = "Beverages", 
                UnitPrice = 18.0000M, UnitsInStock = 20 },
              new { ProductID = 36, ProductName = "Inlagd Sill", Category = "Seafood", 
                UnitPrice = 19.0000M, UnitsInStock = 112 },
              new { ProductID = 37, ProductName = "Gravad lax", Category = "Seafood", 
                UnitPrice = 26.0000M, UnitsInStock = 11 },
              new { ProductID = 38, ProductName = "Côte de Blaye", Category = "Beverages", 
                UnitPrice = 263.5000M, UnitsInStock = 17 },
              new { ProductID = 39, ProductName = "Chartreuse verte", Category = "Beverages", 
                UnitPrice = 18.0000M, UnitsInStock = 69 },
              new { ProductID = 40, ProductName = "Boston Crab Meat", Category = "Seafood", 
                UnitPrice = 18.4000M, UnitsInStock = 123 },
              new { ProductID = 41, ProductName = "Jack's New England Clam Chowder", Category = "Seafood", 
                UnitPrice = 9.6500M, UnitsInStock = 85 },
              new { ProductID = 42, ProductName = "Singaporean Hokkien Fried Mee", Category = "Grains/Cereals", 
                UnitPrice = 14.0000M, UnitsInStock = 26 },
              new { ProductID = 43, ProductName = "Ipoh Coffee", Category = "Beverages", 
                UnitPrice = 46.0000M, UnitsInStock = 17 },
              new { ProductID = 44, ProductName = "Gula Malacca", Category = "Condiments", 
                UnitPrice = 19.4500M, UnitsInStock = 27 },
              new { ProductID = 45, ProductName = "Rogede sild", Category = "Seafood", 
                UnitPrice = 9.5000M, UnitsInStock = 5 },
              new { ProductID = 46, ProductName = "Spegesild", Category = "Seafood", 
                UnitPrice = 12.0000M, UnitsInStock = 95 },
              new { ProductID = 47, ProductName = "Zaanse koeken", Category = "Confections", 
                UnitPrice = 9.5000M, UnitsInStock = 36 },
              new { ProductID = 48, ProductName = "Chocolade", Category = "Confections", 
                UnitPrice = 12.7500M, UnitsInStock = 15 },
              new { ProductID = 49, ProductName = "Maxilaku", Category = "Confections", 
                UnitPrice = 20.0000M, UnitsInStock = 10 },
              new { ProductID = 50, ProductName = "Valkoinen suklaa", Category = "Confections", 
                UnitPrice = 16.2500M, UnitsInStock = 65 },
              new { ProductID = 51, ProductName = "Manjimup Dried Apples", Category = "Produce", 
                UnitPrice = 53.0000M, UnitsInStock = 20 },
              new { ProductID = 52, ProductName = "Filo Mix", Category = "Grains/Cereals", 
                UnitPrice = 7.0000M, UnitsInStock = 38 },
              new { ProductID = 53, ProductName = "Perth Pasties", Category = "Meat/Poultry", 
                UnitPrice = 32.8000M, UnitsInStock = 0 },
              new { ProductID = 54, ProductName = "Tourtière", Category = "Meat/Poultry", 
                UnitPrice = 7.4500M, UnitsInStock = 21 },
              new { ProductID = 55, ProductName = "Pâté chinois", Category = "Meat/Poultry", 
                UnitPrice = 24.0000M, UnitsInStock = 115 },
              new { ProductID = 56, ProductName = "Gnocchi di nonna Alice", Category = "Grains/Cereals", 
                UnitPrice = 38.0000M, UnitsInStock = 21 },
              new { ProductID = 57, ProductName = "Ravioli Angelo", Category = "Grains/Cereals", 
                UnitPrice = 19.5000M, UnitsInStock = 36 },
              new { ProductID = 58, ProductName = "Escargots de Bourgogne", Category = "Seafood", 
                UnitPrice = 13.2500M, UnitsInStock = 62 },
              new { ProductID = 59, ProductName = "Raclette Courdavault", Category = "Dairy Products", 
                UnitPrice = 55.0000M, UnitsInStock = 79 },
              new { ProductID = 60, ProductName = "Camembert Pierrot", Category = "Dairy Products", 
                UnitPrice = 34.0000M, UnitsInStock = 19 },
              new { ProductID = 61, ProductName = "Sirop d'érable", Category = "Condiments", 
                UnitPrice = 28.5000M, UnitsInStock = 113 },
              new { ProductID = 62, ProductName = "Tarte au sucre", Category = "Confections", 
                UnitPrice = 49.3000M, UnitsInStock = 17 },
              new { ProductID = 63, ProductName = "Vegie-spread", Category = "Condiments", 
                UnitPrice = 43.9000M, UnitsInStock = 24 },
              new { ProductID = 64, ProductName = "Wimmers gute Semmelknödel", Category = "Grains/Cereals", 
                UnitPrice = 33.2500M, UnitsInStock = 22 },
              new { ProductID = 65, ProductName = "Louisiana Fiery Hot Pepper Sauce", Category = "Condiments", 
                UnitPrice = 21.0500M, UnitsInStock = 76 },
              new { ProductID = 66, ProductName = "Louisiana Hot Spiced Okra", Category = "Condiments", 
                UnitPrice = 17.0000M, UnitsInStock = 4 },
              new { ProductID = 67, ProductName = "Laughing Lumberjack Lager", Category = "Beverages", 
                UnitPrice = 14.0000M, UnitsInStock = 52 },
              new { ProductID = 68, ProductName = "Scottish Longbreads", Category = "Confections", 
                UnitPrice = 12.5000M, UnitsInStock = 6 },
              new { ProductID = 69, ProductName = "Gudbrandsdalsost", Category = "Dairy Products", 
                UnitPrice = 36.0000M, UnitsInStock = 26 },
              new { ProductID = 70, ProductName = "Outback Lager", Category = "Beverages", 
                UnitPrice = 15.0000M, UnitsInStock = 15 },
              new { ProductID = 71, ProductName = "Flotemysost", Category = "Dairy Products", 
                UnitPrice = 21.5000M, UnitsInStock = 26 },
              new { ProductID = 72, ProductName = "Mozzarella di Giovanni", Category = "Dairy Products", 
                UnitPrice = 34.8000M, UnitsInStock = 14 },
              new { ProductID = 73, ProductName = "Röd Kaviar", Category = "Seafood", 
                UnitPrice = 15.0000M, UnitsInStock = 101 },
              new { ProductID = 74, ProductName = "Longlife Tofu", Category = "Produce", 
                UnitPrice = 10.0000M, UnitsInStock = 4 },
              new { ProductID = 75, ProductName = "Rhönbräu Klosterbier", Category = "Beverages", 
                UnitPrice = 7.7500M, UnitsInStock = 125 },
              new { ProductID = 76, ProductName = "Lakkalikööri", Category = "Beverages", 
                UnitPrice = 18.0000M, UnitsInStock = 57 },
              new { ProductID = 77, ProductName = "Original Frankfurter grüne Soße", Category = "Condiments", 
                UnitPrice = 13.0000M, UnitsInStock = 32 }
            };

            foreach (var x in productList) {
                table.Rows.Add(new object[] {x.ProductID, x.ProductName, x.Category, x.UnitPrice, x.UnitsInStock});
            }

            return table;
        }
    }

    public class Customer {
        public Customer(string customerID, string companyName) {
            CustomerID = customerID;
            CompanyName = companyName;
            Orders = new Order[10];
        }

        public Customer() {}

        public string CustomerID;
        public string CompanyName;
        public string Address;
        public string City;
        public string Region;
        public string PostalCode;
        public string Country;
        public string Phone;
        public string Fax;
        public Order[] Orders;
    }

    public class Order  {
        public Order(int orderID, DateTime orderDate, decimal total) {
            OrderID = orderID;
            OrderDate = orderDate;
            Total = total;
        }

        public Order() {}

        public int OrderID;
        public DateTime OrderDate;
        public decimal Total;
    }

    public class Pet {
        private string name;
        private string owner;

        public string Name {
            set {name = value;}
            get {return name;}
        }

        public string Owner {
            set {owner = value;}
            get {return owner;}
        }
    }

    public class Dog : Pet{
        private string breed;

        public string Breed {
            set {breed = value;}
            get {return breed;}
        }
    }

    public class ShowDog  : Dog{
        private int ranking;

        public int Ranking {
            set {ranking = value;}
            get {return ranking;}
        }
    }

#endregion
}
