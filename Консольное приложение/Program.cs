using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;

namespace WarehouseManagementSystem
{
    // Класс для представления товара
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Barcode { get; set; }
        public string Category { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int MinStockLevel { get; set; }
        public int MaxStockLevel { get; set; }
        public string Location { get; set; } // Стеллаж, секция, ячейка
        public string Supplier { get; set; }
        public DateTime LastRestockDate { get; set; }

        public override string ToString()
        {
            return $"ID: {Id,-3} | {Name,-25} | Штрих-код: {Barcode,-15} | Категория: {Category,-12} | Цена: {UnitPrice,8:C} | Кол-во: {Quantity,4} | Место: {Location,-10}";
        }
    }

    // Класс для представления приходного документа
    public class ReceiptDocument
    {
        public int Id { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string Supplier { get; set; }
        public string DocumentNumber { get; set; } // Номер накладной
        public List<ReceiptItem> Items { get; set; }
        public decimal TotalAmount { get; set; }
        public string WarehouseManager { get; set; }

        public ReceiptDocument()
        {
            Items = new List<ReceiptItem>();
            ReceiptDate = DateTime.Now;
        }

        public override string ToString()
        {
            return $"Приход #{Id} | Дата: {ReceiptDate:dd.MM.yyyy} | Поставщик: {Supplier,-20} | Сумма: {TotalAmount:C} | Ответственный: {WarehouseManager}";
        }
    }

    // Класс для элемента приходного документа
    public class ReceiptItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public string Location { get; set; }

        public override string ToString()
        {
            return $"  {Product.Name,-25} x{Quantity,4} | Цена: {UnitPrice,8:C} | Итого: {Subtotal,10:C} | Место: {Location}";
        }
    }

    // Класс для представления расходного документа
    public class ExpenditureDocument
    {
        public int Id { get; set; }
        public DateTime ExpenditureDate { get; set; }
        public string Customer { get; set; }
        public string DocumentNumber { get; set; }
        public List<ExpenditureItem> Items { get; set; }
        public decimal TotalAmount { get; set; }
        public string Purpose { get; set; } // Назначение (продажа, списание, перемещение)
        public string ResponsiblePerson { get; set; }

        public ExpenditureDocument()
        {
            Items = new List<ExpenditureItem>();
            ExpenditureDate = DateTime.Now;
        }

        public override string ToString()
        {
            return $"Расход #{Id} | Дата: {ExpenditureDate:dd.MM.yyyy} | Получатель: {Customer,-20} | Сумма: {TotalAmount:C} | Назначение: {Purpose}";
        }
    }

    // Класс для элемента расходного документа
    public class ExpenditureItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public string FromLocation { get; set; }

        public override string ToString()
        {
            return $"  {Product.Name,-25} x{Quantity,4} | Цена: {UnitPrice,8:C} | Итого: {Subtotal,10:C} | Из: {FromLocation}";
        }
    }

    // Класс для управления складом
    public class WarehouseLocation
    {
        public string Zone { get; set; } // Зона (A, B, C)
        public string Rack { get; set; } // Стеллаж (A1, A2, B1...)
        public string Section { get; set; } // Секция (1, 2, 3...)
        public string Cell { get; set; } // Ячейка (01, 02, 03...)
        public Product OccupiedBy { get; set; }
        public bool IsOccupied => OccupiedBy != null;
        public double Capacity { get; set; } // В условных единицах

        public string FullLocation => $"{Zone}-{Rack}-{Section}-{Cell}";

        public override string ToString()
        {
            return $"{FullLocation,-15} | Занято: {(IsOccupied ? "Да" : "Нет"),-5} | Товар: {(IsOccupied ? OccupiedBy.Name : "Свободно"),-20} | Вместимость: {Capacity}";
        }
    }

    // Класс для проведения инвентаризации
    public class InventoryCheck
    {
        public int Id { get; set; }
        public DateTime CheckDate { get; set; }
        public string Inspector { get; set; }
        public List<InventoryItem> CheckedItems { get; set; }
        public string Status { get; set; } // В процессе, завершена
        public string Notes { get; set; }

        public InventoryCheck()
        {
            CheckedItems = new List<InventoryItem>();
            CheckDate = DateTime.Now;
            Status = "В процессе";
        }

        public override string ToString()
        {
            return $"Инвентаризация #{Id} | Дата: {CheckDate:dd.MM.yyyy} | Проверяющий: {Inspector} | Статус: {Status}";
        }
    }

    // Класс для элемента инвентаризации
    public class InventoryItem
    {
        public Product Product { get; set; }
        public int SystemQuantity { get; set; }
        public int ActualQuantity { get; set; }
        public int Difference => ActualQuantity - SystemQuantity;
        public bool IsMatch => SystemQuantity == ActualQuantity;

        public override string ToString()
        {
            string status = IsMatch ? "✓" : Difference > 0 ? $"▲+{Difference}" : $"▼{Difference}";
            return $"  {Product.Name,-25} | Система: {SystemQuantity,4} | Факт: {ActualQuantity,4} | {status}";
        }
    }

    // Главный класс приложения
    public class WarehouseManagementSystem
    {
        private List<Product> products;
        private List<ReceiptDocument> receipts;
        private List<ExpenditureDocument> expenditures;
        private List<WarehouseLocation> locations;
        private List<InventoryCheck> inventoryChecks;
        private int nextProductId = 1;
        private int nextReceiptId = 1;
        private int nextExpenditureId = 1;
        private int nextInventoryCheckId = 1;

        public WarehouseManagementSystem()
        {
            products = new List<Product>();
            receipts = new List<ReceiptDocument>();
            expenditures = new List<ExpenditureDocument>();
            locations = new List<WarehouseLocation>();
            inventoryChecks = new List<InventoryCheck>();
            InitializeSampleData();
        }

        // Инициализация примерных данных
        private void InitializeSampleData()
        {
            // Инициализация мест хранения
            InitializeLocations();

            // Добавляем примеры товаров
            products.Add(new Product
            {
                Id = nextProductId++,
                Name = "Ноутбук Lenovo ThinkPad",
                Barcode = "5901234123457",
                Category = "Электроника",
                UnitPrice = 75000m,
                Quantity = 25,
                MinStockLevel = 10,
                MaxStockLevel = 50,
                Location = "A-1-2-01",
                Supplier = "ООО 'ТехноСити'",
                LastRestockDate = DateTime.Now.AddDays(-5)
            });

            products.Add(new Product
            {
                Id = nextProductId++,
                Name = "Монитор Dell 24\"",
                Barcode = "5901234123458",
                Category = "Электроника",
                UnitPrice = 15000m,
                Quantity = 40,
                MinStockLevel = 15,
                MaxStockLevel = 60,
                Location = "A-1-3-02",
                Supplier = "ООО 'МониторГрупп'",
                LastRestockDate = DateTime.Now.AddDays(-3)
            });

            products.Add(new Product
            {
                Id = nextProductId++,
                Name = "Клавиатура механическая",
                Barcode = "5901234123459",
                Category = "Комплектующие",
                UnitPrice = 3500m,
                Quantity = 100,
                MinStockLevel = 50,
                MaxStockLevel = 200,
                Location = "B-2-1-03",
                Supplier = "ИП 'Компьютерные решения'",
                LastRestockDate = DateTime.Now.AddDays(-10)
            });

            products.Add(new Product
            {
                Id = nextProductId++,
                Name = "Мышь беспроводная",
                Barcode = "5901234123460",
                Category = "Комплектующие",
                UnitPrice = 1200m,
                Quantity = 150,
                MinStockLevel = 75,
                MaxStockLevel = 300,
                Location = "B-2-1-04",
                Supplier = "ООО 'Периферия+'",
                LastRestockDate = DateTime.Now.AddDays(-7)
            });

            products.Add(new Product
            {
                Id = nextProductId++,
                Name = "Блок питания 500W",
                Barcode = "5901234123461",
                Category = "Комплектующие",
                UnitPrice = 4500m,
                Quantity = 35,
                MinStockLevel = 20,
                MaxStockLevel = 80,
                Location = "C-3-2-01",
                Supplier = "ООО 'ЭнергоСистемы'",
                LastRestockDate = DateTime.Now.AddDays(-15)
            });

            // Занимаем места хранения товарами
            UpdateLocationOccupancy();
        }

        private void InitializeLocations()
        {
            // Создаем сетку склада: 3 зоны, по 5 стеллажей, по 3 секции, по 5 ячеек
            for (char zone = 'A'; zone <= 'C'; zone++)
            {
                for (int rack = 1; rack <= 5; rack++)
                {
                    for (int section = 1; section <= 3; section++)
                    {
                        for (int cell = 1; cell <= 5; cell++)
                        {
                            locations.Add(new WarehouseLocation
                            {
                                Zone = zone.ToString(),
                                Rack = rack.ToString(),
                                Section = section.ToString(),
                                Cell = cell.ToString("D2"),
                                Capacity = 100
                            });
                        }
                    }
                }
            }
        }

        private void UpdateLocationOccupancy()
        {
            // Освобождаем все места
            foreach (var location in locations)
            {
                location.OccupiedBy = null;
            }

            // Занимаем места товарами
            foreach (var product in products)
            {
                if (!string.IsNullOrEmpty(product.Location))
                {
                    var location = locations.FirstOrDefault(l => l.FullLocation == product.Location);
                    if (location != null)
                    {
                        location.OccupiedBy = product;
                    }
                }
            }
        }

        // Главное меню
        public void Run()
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("══════════════════════════════════════════════════════════════════════════");
                Console.WriteLine("          СИСТЕМА УПРАВЛЕНИЯ ЛОГИСТИЧЕСКИМ СКЛАДОМ");
                Console.WriteLine("══════════════════════════════════════════════════════════════════════════");
                Console.WriteLine();
                Console.WriteLine("1. Управление каталогом товаров");
                Console.WriteLine("2. Управление местами хранения");
                Console.WriteLine("3. Приходная документация");
                Console.WriteLine("4. Расходная документация");
                Console.WriteLine("5. Инвентаризация");
                Console.WriteLine("6. Отчёты и аналитика");
                Console.WriteLine("7. Выход");
                Console.WriteLine();
                Console.Write("Выберите пункт меню (1-7): ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        ProductManagement();
                        break;
                    case "2":
                        LocationManagement();
                        break;
                    case "3":
                        ReceiptManagement();
                        break;
                    case "4":
                        ExpenditureManagement();
                        break;
                    case "5":
                        InventoryManagement();
                        break;
                    case "6":
                        Reports();
                        break;
                    case "7":
                        running = false;
                        Console.WriteLine("До свидания!");
                        break;
                    default:
                        Console.WriteLine("Неверный выбор. Нажмите Enter для продолжения...");
                        Console.ReadLine();
                        break;
                }
            }
        }

        // Управление товарами
        private void ProductManagement()
        {
            bool managing = true;
            while (managing)
            {
                Console.Clear();
                Console.WriteLine("══════════════════════════════════════════════════════════════════════════");
                Console.WriteLine("                 УПРАВЛЕНИЕ КАТАЛОГОМ ТОВАРОВ");
                Console.WriteLine("══════════════════════════════════════════════════════════════════════════");
                Console.WriteLine();
                Console.WriteLine("1. Просмотреть все товары");
                Console.WriteLine("2. Добавить новый товар");
                Console.WriteLine("3. Обновить данные товара");
                Console.WriteLine("4. Удалить товар");
                Console.WriteLine("5. Поиск товара по штрих-коду");
                Console.WriteLine("6. Проверить товары с низким запасом");
                Console.WriteLine("7. Проверить переполненные места хранения");
                Console.WriteLine("8. Вернуться в главное меню");
                Console.WriteLine();
                Console.Write("Выберите действие (1-8): ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        ViewAllProducts();
                        break;
                    case "2":
                        AddProduct();
                        break;
                    case "3":
                        UpdateProduct();
                        break;
                    case "4":
                        DeleteProduct();
                        break;
                    case "5":
                        SearchByBarcode();
                        break;
                    case "6":
                        CheckLowStock();
                        break;
                    case "7":
                        CheckOverstock();
                        break;
                    case "8":
                        managing = false;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор.");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private void ViewAllProducts()
        {
            Console.Clear();
            Console.WriteLine("══════════════════════════════════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine("                                               ВСЕ ТОВАРЫ");
            Console.WriteLine("══════════════════════════════════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine();

            if (products.Count == 0)
            {
                Console.WriteLine("Товаров нет.");
            }
            else
            {
                Console.WriteLine($"Всего товаров: {products.Count}");
                Console.WriteLine($"Общая стоимость запасов: {products.Sum(p => p.UnitPrice * p.Quantity):C}");
                Console.WriteLine();

                // Группировка по категориям
                var grouped = products.GroupBy(p => p.Category);
                foreach (var group in grouped)
                {
                    Console.WriteLine($"\n📁 КАТЕГОРИЯ: {group.Key}");
                    Console.WriteLine(new string('=', 100));
                    Console.WriteLine($"{"Название",-25} | {"Штрих-код",-15} | {"Кол-во",6} | {"Цена",10} | {"Место",-10} | {"Поставщик",-20}");
                    Console.WriteLine(new string('-', 100));
                    foreach (var product in group)
                    {
                        Console.WriteLine($"{product.Name,-25} | {product.Barcode,-15} | {product.Quantity,6} | {product.UnitPrice,10:C} | {product.Location,-10} | {product.Supplier,-20}");
                    }
                }
            }

            Console.WriteLine("\nНажмите Enter для продолжения...");
            Console.ReadLine();
        }

        private void AddProduct()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("                    ДОБАВИТЬ ТОВАР");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine();

            Console.Write("Введите название товара: ");
            string name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Название не может быть пустым.");
                Console.ReadLine();
                return;
            }

            Console.Write("Введите штрих-код: ");
            string barcode = Console.ReadLine();

            // Проверка уникальности штрих-кода
            if (products.Any(p => p.Barcode == barcode))
            {
                Console.WriteLine("Товар с таким штрих-кодом уже существует.");
                Console.ReadLine();
                return;
            }

            Console.Write("Введите категорию: ");
            string category = Console.ReadLine();

            Console.Write("Введите цену за единицу: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price < 0)
            {
                Console.WriteLine("Неверная цена.");
                Console.ReadLine();
                return;
            }

            Console.Write("Введите начальное количество: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity < 0)
            {
                Console.WriteLine("Неверное количество.");
                Console.ReadLine();
                return;
            }

            Console.Write("Введите минимальный запас: ");
            if (!int.TryParse(Console.ReadLine(), out int minStock) || minStock < 0)
            {
                Console.WriteLine("Неверное значение.");
                Console.ReadLine();
                return;
            }

            Console.Write("Введите максимальный запас: ");
            if (!int.TryParse(Console.ReadLine(), out int maxStock) || maxStock < minStock)
            {
                Console.WriteLine("Неверное значение.");
                Console.ReadLine();
                return;
            }

            Console.Write("Введите место хранения (формат: A-1-1-01): ");
            string location = Console.ReadLine();

            Console.Write("Введите поставщика: ");
            string supplier = Console.ReadLine();

            var newProduct = new Product
            {
                Id = nextProductId++,
                Name = name,
                Barcode = barcode,
                Category = category,
                UnitPrice = price,
                Quantity = quantity,
                MinStockLevel = minStock,
                MaxStockLevel = maxStock,
                Location = location,
                Supplier = supplier,
                LastRestockDate = DateTime.Now
            };

            products.Add(newProduct);
            UpdateLocationOccupancy();

            Console.WriteLine($"\n✓ Товар '{name}' успешно добавлен.");
            Console.ReadLine();
        }

        private void UpdateProduct()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("                  ОБНОВИТЬ ДАННЫЕ ТОВАРА");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine();

            Console.Write("Введите ID товара для обновления: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Неверный ID.");
                Console.ReadLine();
                return;
            }

            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                Console.WriteLine("Товар не найден.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"\nТекущие данные: {product}");
            Console.WriteLine();

            Console.Write("Введите новое название (Enter для пропуска): ");
            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
                product.Name = name;

            Console.Write("Введите новую категорию (Enter для пропуска): ");
            string category = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(category))
                product.Category = category;

            Console.Write("Введите новую цену (Enter для пропуска): ");
            string priceInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(priceInput) && decimal.TryParse(priceInput, out decimal price))
                product.UnitPrice = price;

            Console.Write("Введите новое количество (Enter для пропуска): ");
            string qtyInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(qtyInput) && int.TryParse(qtyInput, out int quantity))
                product.Quantity = quantity;

            Console.Write("Введите новое место хранения (Enter для пропуска): ");
            string location = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(location))
                product.Location = location;

            Console.Write("Введите нового поставщика (Enter для пропуска): ");
            string supplier = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(supplier))
                product.Supplier = supplier;

            UpdateLocationOccupancy();
            Console.WriteLine($"\n✓ Товар успешно обновлён.");
            Console.ReadLine();
        }

        private void DeleteProduct()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("                    УДАЛИТЬ ТОВАР");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine();

            Console.Write("Введите ID товара для удаления: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Неверный ID.");
                Console.ReadLine();
                return;
            }

            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                Console.WriteLine("Товар не найден.");
                Console.ReadLine();
                return;
            }

            if (product.Quantity > 0)
            {
                Console.WriteLine($"Внимание! На складе есть {product.Quantity} единиц этого товара.");
                Console.Write("Вы уверены, что хотите удалить товар? (д/н): ");
                if (Console.ReadLine().ToLower() != "д")
                {
                    Console.WriteLine("✗ Удаление отменено.");
                    Console.ReadLine();
                    return;
                }
            }

            products.Remove(product);
            UpdateLocationOccupancy();
            Console.WriteLine($"\n✓ Товар '{product.Name}' удалён.");
            Console.ReadLine();
        }

        private void SearchByBarcode()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("              ПОИСК ТОВАРА ПО ШТРИХ-КОДУ");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine();

            Console.Write("Введите штрих-код: ");
            string barcode = Console.ReadLine();

            var product = products.FirstOrDefault(p => p.Barcode == barcode);
            if (product == null)
            {
                Console.WriteLine("Товар не найден.");
            }
            else
            {
                Console.WriteLine("\nНайденный товар:");
                Console.WriteLine(new string('-', 80));
                Console.WriteLine(product);
                Console.WriteLine($"Минимальный запас: {product.MinStockLevel}");
                Console.WriteLine($"Максимальный запас: {product.MaxStockLevel}");
                Console.WriteLine($"Последнее поступление: {product.LastRestockDate:dd.MM.yyyy}");
                Console.WriteLine($"Стоимость запаса: {product.UnitPrice * product.Quantity:C}");
            }

            Console.WriteLine("\nНажмите Enter для продолжения...");
            Console.ReadLine();
        }

        private void CheckLowStock()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine("               ТОВАРЫ С НИЗКИМ ЗАПАСОМ (ниже минимального уровня)");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine();

            var lowStock = products.Where(p => p.Quantity < p.MinStockLevel).OrderBy(p => p.Quantity).ToList();

            if (lowStock.Count == 0)
            {
                Console.WriteLine("✓ Все товары в наличии в нормальном количестве.");
            }
            else
            {
                Console.WriteLine($"{"Название",-25} | {"Текущее",7} | {"Мин",4} | {"Разница",8} | {"Категория",-12}");
                Console.WriteLine(new string('-', 70));
                foreach (var product in lowStock)
                {
                    int difference = product.MinStockLevel - product.Quantity;
                    Console.WriteLine($"⚠️  {product.Name,-25} | {product.Quantity,7} | {product.MinStockLevel,4} | -{difference,7} | {product.Category,-12}");
                }
                Console.WriteLine($"\nВсего товаров с низким запасом: {lowStock.Count}");
            }

            Console.WriteLine("\nНажмите Enter для продолжения...");
            Console.ReadLine();
        }

        private void CheckOverstock()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine("            ПЕРЕПОЛНЕННЫЕ МЕСТА ХРАНЕНИЯ (выше максимального уровня)");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine();

            var overstock = products.Where(p => p.Quantity > p.MaxStockLevel).OrderByDescending(p => p.Quantity).ToList();

            if (overstock.Count == 0)
            {
                Console.WriteLine("✓ Нет переполненных мест хранения.");
            }
            else
            {
                Console.WriteLine($"{"Название",-25} | {"Текущее",7} | {"Макс",4} | {"Превышение",10} | {"Место",-10}");
                Console.WriteLine(new string('-', 70));
                foreach (var product in overstock)
                {
                    int excess = product.Quantity - product.MaxStockLevel;
                    Console.WriteLine($"⚠️  {product.Name,-25} | {product.Quantity,7} | {product.MaxStockLevel,4} | +{excess,9} | {product.Location,-10}");
                }
                Console.WriteLine($"\nВсего переполненных позиций: {overstock.Count}");
            }

            Console.WriteLine("\nНажмите Enter для продолжения...");
            Console.ReadLine();
        }

        // Управление местами хранения
        private void LocationManagement()
        {
            bool managing = true;
            while (managing)
            {
                Console.Clear();
                Console.WriteLine("══════════════════════════════════════════════════════════════════════════");
                Console.WriteLine("               УПРАВЛЕНИЕ МЕСТАМИ ХРАНЕНИЯ");
                Console.WriteLine("══════════════════════════════════════════════════════════════════════════");
                Console.WriteLine();
                Console.WriteLine("1. Просмотреть все места хранения");
                Console.WriteLine("2. Просмотреть занятые места");
                Console.WriteLine("3. Просмотреть свободные места");
                Console.WriteLine("4. Оптимизировать размещение");
                Console.WriteLine("5. Найти товар по месту хранения");
                Console.WriteLine("6. Вернуться в главное меню");
                Console.WriteLine();
                Console.Write("Выберите действие (1-6): ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        ViewAllLocations();
                        break;
                    case "2":
                        ViewOccupiedLocations();
                        break;
                    case "3":
                        ViewFreeLocations();
                        break;
                    case "4":
                        OptimizeStorage();
                        break;
                    case "5":
                        FindProductByLocation();
                        break;
                    case "6":
                        managing = false;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор.");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private void ViewAllLocations()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine("                                   ВСЕ МЕСТА ХРАНЕНИЯ");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine();

            Console.WriteLine($"Всего мест: {locations.Count}");
            Console.WriteLine($"Занято: {locations.Count(l => l.IsOccupied)}");
            Console.WriteLine($"Свободно: {locations.Count(l => !l.IsOccupied)}");
            Console.WriteLine($"Коэффициент заполнения: {((double)locations.Count(l => l.IsOccupied) / locations.Count * 100):F1}%");
            Console.WriteLine();

            // Группировка по зонам
            var grouped = locations.GroupBy(l => l.Zone);
            foreach (var zoneGroup in grouped)
            {
                Console.WriteLine($"\n📍 ЗОНА {zoneGroup.Key}:");
                Console.WriteLine(new string('-', 80));

                var rackGroups = zoneGroup.GroupBy(l => l.Rack);
                foreach (var rackGroup in rackGroups)
                {
                    Console.WriteLine($"  Стеллаж {rackGroup.Key}:");
                    foreach (var location in rackGroup.OrderBy(l => l.Section).ThenBy(l => l.Cell))
                    {
                        string status = location.IsOccupied ? "[ЗАНЯТО]" : "[СВОБОДНО]";
                        string productName = location.IsOccupied ? location.OccupiedBy.Name : "";
                        Console.WriteLine($"    {location.Section}-{location.Cell}: {status} {productName}");
                    }
                    Console.WriteLine();
                }
            }

            Console.WriteLine("\nНажмите Enter для продолжения...");
            Console.ReadLine();
        }

        private void ViewOccupiedLocations()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine("                         ЗАНЯТЫЕ МЕСТА ХРАНЕНИЯ");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine();

            var occupied = locations.Where(l => l.IsOccupied).OrderBy(l => l.FullLocation).ToList();

            if (occupied.Count == 0)
            {
                Console.WriteLine("Нет занятых мест.");
            }
            else
            {
                Console.WriteLine($"Занято мест: {occupied.Count}");
                Console.WriteLine();

                Console.WriteLine($"{"Место",-15} | {"Товар",-25} | {"Кол-во",6} | {"Категория",-12}");
                Console.WriteLine(new string('-', 70));
                foreach (var location in occupied)
                {
                    Console.WriteLine($"{location.FullLocation,-15} | {location.OccupiedBy.Name,-25} | {location.OccupiedBy.Quantity,6} | {location.OccupiedBy.Category,-12}");
                }
            }

            Console.WriteLine("\nНажмите Enter для продолжения...");
            Console.ReadLine();
        }

        private void ViewFreeLocations()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine("                         СВОБОДНЫЕ МЕСТА ХРАНЕНИЯ");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine();

            var free = locations.Where(l => !l.IsOccupied).OrderBy(l => l.FullLocation).ToList();

            if (free.Count == 0)
            {
                Console.WriteLine("Нет свободных мест.");
            }
            else
            {
                Console.WriteLine($"Свободно мест: {free.Count}");
                Console.WriteLine();

                Console.WriteLine($"{"Место",-15} | {"Вместимость",12}");
                Console.WriteLine(new string('-', 30));
                foreach (var location in free.Take(50)) // Показываем только первые 50
                {
                    Console.WriteLine($"{location.FullLocation,-15} | {location.Capacity,12}");
                }

                if (free.Count > 50)
                {
                    Console.WriteLine($"\n... и ещё {free.Count - 50} свободных мест");
                }
            }

            Console.WriteLine("\nНажмите Enter для продолжения...");
            Console.ReadLine();
        }

        private void OptimizeStorage()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine("                     ОПТИМИЗАЦИЯ РАЗМЕЩЕНИЯ ТОВАРОВ");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine();

            Console.WriteLine("Доступные функции оптимизации:");
            Console.WriteLine("1. Сгруппировать товары по категориям");
            Console.WriteLine("2. Освободить фрагментированные места");
            Console.WriteLine("3. Переместить товары с низким спросом в дальние зоны");
            Console.WriteLine("4. Автоматическая оптимизация");
            Console.Write("\nВыберите функцию (1-4): ");

            string choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    OptimizeByCategory();
                    break;
                case "2":
                    Console.WriteLine("Функция в разработке...");
                    break;
                case "3":
                    Console.WriteLine("Функция в разработке...");
                    break;
                case "4":
                    AutoOptimize();
                    break;
                default:
                    Console.WriteLine("Неверный выбор.");
                    break;
            }

            Console.WriteLine("\nНажмите Enter для продолжения...");
            Console.ReadLine();
        }

        private void OptimizeByCategory()
        {
            Console.WriteLine("Оптимизация по категориям...");

            // Группируем товары по категориям
            var categories = products.Select(p => p.Category).Distinct();
            var freeLocations = locations.Where(l => !l.IsOccupied).ToList();

            int movedCount = 0;
            int zoneIndex = 0;
            char[] zones = { 'A', 'B', 'C', 'D', 'E' };

            foreach (var category in categories)
            {
                var categoryProducts = products.Where(p => p.Category == category).ToList();

                // Назначаем каждую категорию в свою зону
                if (zoneIndex < zones.Length)
                {
                    char targetZone = zones[zoneIndex];
                    var zoneLocations = freeLocations.Where(l => l.Zone == targetZone.ToString()).ToList();

                    int locationIndex = 0;
                    foreach (var product in categoryProducts)
                    {
                        if (locationIndex < zoneLocations.Count)
                        {
                            product.Location = zoneLocations[locationIndex].FullLocation;
                            locationIndex++;
                            movedCount++;
                        }
                    }

                    zoneIndex++;
                }
            }

            UpdateLocationOccupancy();
            Console.WriteLine($"✓ Перемещено товаров: {movedCount}");
            Console.WriteLine($"Товары сгруппированы по {Math.Min(zoneIndex, zones.Length)} зонам");
        }

        private void AutoOptimize()
        {
            Console.WriteLine("Автоматическая оптимизация запущена...");

            // 1. Освобождаем все места
            UpdateLocationOccupancy();

            // 2. Сортируем товары по частоте использования (здесь просто по категории)
            var sortedProducts = products.OrderBy(p => p.Category)
                                          .ThenByDescending(p => p.Quantity) // Большие партии в начало
                                          .ToList();

            // 3. Распределяем по зонам
            var freeLocations = locations.OrderBy(l => l.Zone)
                                          .ThenBy(l => l.Rack)
                                          .ThenBy(l => l.Section)
                                          .ThenBy(l => l.Cell)
                                          .ToList();

            int placedCount = 0;
            for (int i = 0; i < sortedProducts.Count && i < freeLocations.Count; i++)
            {
                sortedProducts[i].Location = freeLocations[i].FullLocation;
                placedCount++;
            }

            UpdateLocationOccupancy();
            Console.WriteLine($"✓ Оптимизация завершена");
            Console.WriteLine($"Размещено товаров: {placedCount}/{products.Count}");
            Console.WriteLine($"Использовано мест: {placedCount}/{locations.Count}");
            Console.WriteLine($"Коэффициент заполнения: {((double)placedCount / locations.Count * 100):F1}%");
        }

        private void FindProductByLocation()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("         ПОИСК ТОВАРА ПО МЕСТУ ХРАНЕНИЯ");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine();

            Console.Write("Введите место хранения (формат: A-1-1-01 или A-1-1): ");
            string locationInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(locationInput))
            {
                Console.WriteLine("Место не указано.");
                Console.ReadLine();
                return;
            }

            // Ищем точное совпадение или частичное
            var foundProducts = products.Where(p =>
                p.Location != null &&
                p.Location.StartsWith(locationInput)).ToList();

            if (foundProducts.Count == 0)
            {
                Console.WriteLine("Товары не найдены.");
            }
            else
            {
                Console.WriteLine($"\nНайдено товаров: {foundProducts.Count}");
                Console.WriteLine(new string('-', 80));
                foreach (var product in foundProducts)
                {
                    Console.WriteLine($"{product.Location} → {product.Name} (ID: {product.Id}, Кол-во: {product.Quantity})");
                }
            }

            Console.WriteLine("\nНажмите Enter для продолжения...");
            Console.ReadLine();
        }

        // Управление приходной документацией
        private void ReceiptManagement()
        {
            bool managing = true;
            while (managing)
            {
                Console.Clear();
                Console.WriteLine("══════════════════════════════════════════════════════════════════════════");
                Console.WriteLine("              ПРИХОДНАЯ ДОКУМЕНТАЦИЯ");
                Console.WriteLine("══════════════════════════════════════════════════════════════════════════");
                Console.WriteLine();
                Console.WriteLine("1. Создать приходный документ");
                Console.WriteLine("2. Просмотреть все приходы");
                Console.WriteLine("3. Просмотреть детали прихода");
                Console.WriteLine("4. Вернуться в главное меню");
                Console.WriteLine();
                Console.Write("Выберите действие (1-4): ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        CreateReceipt();
                        break;
                    case "2":
                        ViewAllReceipts();
                        break;
                    case "3":
                        ViewReceiptDetails();
                        break;
                    case "4":
                        managing = false;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор.");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private void CreateReceipt()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("            СОЗДАТЬ ПРИХОДНЫЙ ДОКУМЕНТ");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine();

            var receipt = new ReceiptDocument { Id = nextReceiptId++ };

            Console.Write("Введите номер накладной: ");
            receipt.DocumentNumber = Console.ReadLine();

            Console.Write("Введите поставщика: ");
            receipt.Supplier = Console.ReadLine();

            Console.Write("Введите ответственного кладовщика: ");
            receipt.WarehouseManager = Console.ReadLine();

            Console.WriteLine("\nДобавление товаров в приход:");
            Console.WriteLine("═══════════════════════════════════════════════════════");

            bool addingItems = true;
            while (addingItems)
            {
                Console.WriteLine("\n1. Добавить существующий товар");
                Console.WriteLine("2. Добавить новый товар");
                Console.WriteLine("3. Завершить добавление");
                Console.Write("Выберите действие (1-3): ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        AddExistingProductToReceipt(receipt);
                        break;
                    case "2":
                        AddNewProductToReceipt(receipt);
                        break;
                    case "3":
                        if (receipt.Items.Count == 0)
                        {
                            Console.WriteLine("Приход не может быть пустым.");
                            continue;
                        }
                        addingItems = false;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор.");
                        break;
                }
            }

            receipt.TotalAmount = receipt.Items.Sum(item => item.Subtotal);
            receipts.Add(receipt);

            // Обновляем даты последнего поступления
            foreach (var item in receipt.Items)
            {
                item.Product.LastRestockDate = DateTime.Now;
            }

            Console.WriteLine($"\n✓ Приходный документ #{receipt.Id} создан успешно!");
            Console.WriteLine($"Сумма: {receipt.TotalAmount:C}");
            Console.WriteLine($"Количество позиций: {receipt.Items.Count}");
            Console.ReadLine();
        }

        private void AddExistingProductToReceipt(ReceiptDocument receipt)
        {
            Console.WriteLine("Доступные товары:");
            ViewAllProducts();

            Console.Write("Введите ID товара: ");
            if (!int.TryParse(Console.ReadLine(), out int productId))
            {
                Console.WriteLine("Неверный ID.");
                Console.ReadLine();
                return;
            }

            var product = products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                Console.WriteLine("Товар не найден.");
                Console.ReadLine();
                return;
            }

            Console.Write("Количество: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
            {
                Console.WriteLine("Неверное количество.");
                Console.ReadLine();
                return;
            }

            Console.Write("Цена за единицу (текущая: {0:C}): ", product.UnitPrice);
            if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price < 0)
            {
                price = product.UnitPrice;
            }

            Console.Write("Место хранения (текущее: {0}): ", product.Location);
            string location = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(location))
            {
                location = product.Location;
            }

            var existingItem = receipt.Items.FirstOrDefault(ri => ri.Product.Id == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.Subtotal = existingItem.Quantity * existingItem.UnitPrice;
            }
            else
            {
                receipt.Items.Add(new ReceiptItem
                {
                    Product = product,
                    Quantity = quantity,
                    UnitPrice = price,
                    Subtotal = quantity * price,
                    Location = location
                });
            }

            // Обновляем количество товара на складе
            product.Quantity += quantity;
            if (!string.IsNullOrWhiteSpace(location))
            {
                product.Location = location;
            }

            Console.WriteLine($"✓ {product.Name} добавлен в приход.");
            Console.ReadLine();
        }

        private void AddNewProductToReceipt(ReceiptDocument receipt)
        {
            Console.Write("Введите название товара: ");
            string name = Console.ReadLine();

            Console.Write("Введите штрих-код: ");
            string barcode = Console.ReadLine();

            Console.Write("Введите категорию: ");
            string category = Console.ReadLine();

            Console.Write("Введите цену за единицу: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price < 0)
            {
                Console.WriteLine("Неверная цена.");
                Console.ReadLine();
                return;
            }

            Console.Write("Количество: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
            {
                Console.WriteLine("Неверное количество.");
                Console.ReadLine();
                return;
            }

            Console.Write("Минимальный запас: ");
            if (!int.TryParse(Console.ReadLine(), out int minStock) || minStock < 0)
            {
                minStock = 10;
            }

            Console.Write("Максимальный запас: ");
            if (!int.TryParse(Console.ReadLine(), out int maxStock) || maxStock < minStock)
            {
                maxStock = minStock * 5;
            }

            Console.Write("Место хранения: ");
            string location = Console.ReadLine();

            var newProduct = new Product
            {
                Id = nextProductId++,
                Name = name,
                Barcode = barcode,
                Category = category,
                UnitPrice = price,
                Quantity = quantity,
                MinStockLevel = minStock,
                MaxStockLevel = maxStock,
                Location = location,
                Supplier = receipt.Supplier,
                LastRestockDate = DateTime.Now
            };

            products.Add(newProduct);

            receipt.Items.Add(new ReceiptItem
            {
                Product = newProduct,
                Quantity = quantity,
                UnitPrice = price,
                Subtotal = quantity * price,
                Location = location
            });

            UpdateLocationOccupancy();
            Console.WriteLine($"✓ Новый товар '{name}' создан и добавлен в приход.");
            Console.ReadLine();
        }

        private void ViewAllReceipts()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine("                          ВСЕ ПРИХОДНЫЕ ДОКУМЕНТЫ");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine();

            if (receipts.Count == 0)
            {
                Console.WriteLine("Приходных документов нет.");
            }
            else
            {
                Console.WriteLine($"Всего приходов: {receipts.Count}");
                Console.WriteLine($"Общая сумма: {receipts.Sum(r => r.TotalAmount):C}");
                Console.WriteLine();

                foreach (var receipt in receipts.OrderByDescending(r => r.ReceiptDate))
                {
                    Console.WriteLine(receipt);
                }
            }

            Console.WriteLine("\nНажмите Enter для продолжения...");
            Console.ReadLine();
        }

        private void ViewReceiptDetails()
        {
            Console.Write("Введите ID приходного документа: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Неверный ID.");
                Console.ReadLine();
                return;
            }

            var receipt = receipts.FirstOrDefault(r => r.Id == id);
            if (receipt == null)
            {
                Console.WriteLine("Документ не найден.");
                Console.ReadLine();
                return;
            }

            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine($"                 ДЕТАЛИ ПРИХОДНОГО ДОКУМЕНТА #{receipt.Id}");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine();
            Console.WriteLine($"Дата: {receipt.ReceiptDate:dd.MM.yyyy HH:mm:ss}");
            Console.WriteLine($"Номер накладной: {receipt.DocumentNumber}");
            Console.WriteLine($"Поставщик: {receipt.Supplier}");
            Console.WriteLine($"Ответственный: {receipt.WarehouseManager}");
            Console.WriteLine();
            Console.WriteLine("Товары:");
            Console.WriteLine(new string('-', 87));
            foreach (var item in receipt.Items)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine(new string('-', 87));
            Console.WriteLine($"ИТОГО: {receipt.TotalAmount,80:C}");

            Console.WriteLine("\nНажмите Enter для продолжения...");
            Console.ReadLine();
        }

        // Управление расходной документацией
        private void ExpenditureManagement()
        {
            bool managing = true;
            while (managing)
            {
                Console.Clear();
                Console.WriteLine("══════════════════════════════════════════════════════════════════════════");
                Console.WriteLine("              РАСХОДНАЯ ДОКУМЕНТАЦИЯ");
                Console.WriteLine("══════════════════════════════════════════════════════════════════════════");
                Console.WriteLine();
                Console.WriteLine("1. Создать расходный документ");
                Console.WriteLine("2. Просмотреть все расходы");
                Console.WriteLine("3. Просмотреть детали расхода");
                Console.WriteLine("4. Вернуться в главное меню");
                Console.WriteLine();
                Console.Write("Выберите действие (1-4): ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        CreateExpenditure();
                        break;
                    case "2":
                        ViewAllExpenditures();
                        break;
                    case "3":
                        ViewExpenditureDetails();
                        break;
                    case "4":
                        managing = false;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор.");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private void CreateExpenditure()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("            СОЗДАТЬ РАСХОДНЫЙ ДОКУМЕНТ");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine();

            var expenditure = new ExpenditureDocument { Id = nextExpenditureId++ };

            Console.Write("Введите номер документа: ");
            expenditure.DocumentNumber = Console.ReadLine();

            Console.Write("Введите получателя: ");
            expenditure.Customer = Console.ReadLine();

            Console.WriteLine("\nВыберите назначение:");
            Console.WriteLine("1. Продажа");
            Console.WriteLine("2. Списание (брак, порча)");
            Console.WriteLine("3. Внутреннее использование");
            Console.WriteLine("4. Возврат поставщику");
            Console.WriteLine("5. Перемещение на другой склад");
            Console.Write("Выберите (1-5): ");

            expenditure.Purpose = Console.ReadLine() switch
            {
                "1" => "Продажа",
                "2" => "Списание",
                "3" => "Внутреннее использование",
                "4" => "Возврат поставщику",
                "5" => "Перемещение",
                _ => "Продажа"
            };

            Console.Write("Введите ответственное лицо: ");
            expenditure.ResponsiblePerson = Console.ReadLine();

            Console.WriteLine("\nДобавление товаров в расход:");
            Console.WriteLine("═══════════════════════════════════════════════════════");

            bool addingItems = true;
            while (addingItems)
            {
                ViewAllProducts();

                Console.Write("Введите ID товара (0 для завершения): ");
                if (!int.TryParse(Console.ReadLine(), out int productId) || productId == 0)
                {
                    if (expenditure.Items.Count == 0)
                    {
                        Console.WriteLine("Расход не может быть пустым.");
                        Console.ReadLine();
                        continue;
                    }
                    addingItems = false;
                    break;
                }

                var product = products.FirstOrDefault(p => p.Id == productId);
                if (product == null)
                {
                    Console.WriteLine("Товар не найден.");
                    Console.ReadLine();
                    continue;
                }

                Console.Write("Количество (доступно: {0}): ", product.Quantity);
                if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
                {
                    Console.WriteLine("Неверное количество.");
                    Console.ReadLine();
                    continue;
                }

                if (quantity > product.Quantity)
                {
                    Console.WriteLine($"⚠️  Недостаточно товара. В наличии: {product.Quantity}");
                    Console.ReadLine();
                    continue;
                }

                Console.Write("Цена за единицу (текущая: {0:C}): ", product.UnitPrice);
                if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price < 0)
                {
                    price = product.UnitPrice;
                }

                var existingItem = expenditure.Items.FirstOrDefault(ei => ei.Product.Id == productId);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                    existingItem.Subtotal = existingItem.Quantity * existingItem.UnitPrice;
                }
                else
                {
                    expenditure.Items.Add(new ExpenditureItem
                    {
                        Product = product,
                        Quantity = quantity,
                        UnitPrice = price,
                        Subtotal = quantity * price,
                        FromLocation = product.Location
                    });
                }

                // Уменьшаем количество товара на складе
                product.Quantity -= quantity;

                Console.WriteLine($"✓ {product.Name} добавлен в расход.");
                Console.ReadLine();
            }

            expenditure.TotalAmount = expenditure.Items.Sum(item => item.Subtotal);
            expenditures.Add(expenditure);

            Console.WriteLine($"\n✓ Расходный документ #{expenditure.Id} создан успешно!");
            Console.WriteLine($"Сумма: {expenditure.TotalAmount:C}");
            Console.WriteLine($"Количество позиций: {expenditure.Items.Count}");
            Console.ReadLine();
        }

        private void ViewAllExpenditures()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine("                          ВСЕ РАСХОДНЫЕ ДОКУМЕНТЫ");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine();

            if (expenditures.Count == 0)
            {
                Console.WriteLine("Расходных документов нет.");
            }
            else
            {
                Console.WriteLine($"Всего расходов: {expenditures.Count}");
                Console.WriteLine($"Общая сумма: {expenditures.Sum(e => e.TotalAmount):C}");
                Console.WriteLine();

                foreach (var expenditure in expenditures.OrderByDescending(e => e.ExpenditureDate))
                {
                    Console.WriteLine(expenditure);
                }
            }

            Console.WriteLine("\nНажмите Enter для продолжения...");
            Console.ReadLine();
        }

        private void ViewExpenditureDetails()
        {
            Console.Write("Введите ID расходного документа: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Неверный ID.");
                Console.ReadLine();
                return;
            }

            var expenditure = expenditures.FirstOrDefault(e => e.Id == id);
            if (expenditure == null)
            {
                Console.WriteLine("Документ не найден.");
                Console.ReadLine();
                return;
            }

            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine($"                 ДЕТАЛИ РАСХОДНОГО ДОКУМЕНТА #{expenditure.Id}");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine();
            Console.WriteLine($"Дата: {expenditure.ExpenditureDate:dd.MM.yyyy HH:mm:ss}");
            Console.WriteLine($"Номер документа: {expenditure.DocumentNumber}");
            Console.WriteLine($"Получатель: {expenditure.Customer}");
            Console.WriteLine($"Назначение: {expenditure.Purpose}");
            Console.WriteLine($"Ответственный: {expenditure.ResponsiblePerson}");
            Console.WriteLine();
            Console.WriteLine("Товары:");
            Console.WriteLine(new string('-', 87));
            foreach (var item in expenditure.Items)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine(new string('-', 87));
            Console.WriteLine($"ИТОГО: {expenditure.TotalAmount,80:C}");

            Console.WriteLine("\nНажмите Enter для продолжения...");
            Console.ReadLine();
        }

        // Управление инвентаризацией
        private void InventoryManagement()
        {
            bool managing = true;
            while (managing)
            {
                Console.Clear();
                Console.WriteLine("══════════════════════════════════════════════════════════════════════════");
                Console.WriteLine("                    ИНВЕНТАРИЗАЦИЯ");
                Console.WriteLine("══════════════════════════════════════════════════════════════════════════");
                Console.WriteLine();
                Console.WriteLine("1. Начать новую инвентаризацию");
                Console.WriteLine("2. Продолжить текущую инвентаризацию");
                Console.WriteLine("3. Завершить инвентаризацию");
                Console.WriteLine("4. Просмотреть историю инвентаризаций");
                Console.WriteLine("5. Просмотреть детали инвентаризации");
                Console.WriteLine("6. Вернуться в главное меню");
                Console.WriteLine();
                Console.Write("Выберите действие (1-6): ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        StartInventoryCheck();
                        break;
                    case "2":
                        ContinueInventoryCheck();
                        break;
                    case "3":
                        CompleteInventoryCheck();
                        break;
                    case "4":
                        ViewInventoryHistory();
                        break;
                    case "5":
                        ViewInventoryDetails();
                        break;
                    case "6":
                        managing = false;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор.");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private void StartInventoryCheck()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("            НАЧАТЬ НОВУЮ ИНВЕНТАРИЗАЦИЮ");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine();

            var activeCheck = inventoryChecks.FirstOrDefault(ic => ic.Status == "В процессе");
            if (activeCheck != null)
            {
                Console.WriteLine("⚠️  Уже есть активная инвентаризация!");
                Console.WriteLine($"Инвентаризация #{activeCheck.Id} от {activeCheck.CheckDate:dd.MM.yyyy}");
                Console.Write("Хотите продолжить её? (д/н): ");
                if (Console.ReadLine().ToLower() == "д")
                {
                    ContinueInventoryCheck();
                    return;
                }
            }

            Console.Write("Введите имя проверяющего: ");
            string inspector = Console.ReadLine();

            var newCheck = new InventoryCheck
            {
                Id = nextInventoryCheckId++,
                Inspector = inspector
            };

            // Добавляем все товары в проверку с системным количеством
            foreach (var product in products)
            {
                newCheck.CheckedItems.Add(new InventoryItem
                {
                    Product = product,
                    SystemQuantity = product.Quantity,
                    ActualQuantity = 0 // Пока не посчитано
                });
            }

            inventoryChecks.Add(newCheck);

            Console.WriteLine($"\n✓ Инвентаризация #{newCheck.Id} начата.");
            Console.WriteLine($"Проверяющий: {inspector}");
            Console.WriteLine($"Товаров для проверки: {newCheck.CheckedItems.Count}");
            Console.ReadLine();
        }

        private void ContinueInventoryCheck()
        {
            var activeCheck = inventoryChecks.FirstOrDefault(ic => ic.Status == "В процессе");
            if (activeCheck == null)
            {
                Console.WriteLine("Нет активной инвентаризации.");
                Console.ReadLine();
                return;
            }

            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine($"               ПРОДОЛЖЕНИЕ ИНВЕНТАРИЗАЦИИ #{activeCheck.Id}");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine();

            int uncheckedCount = activeCheck.CheckedItems.Count(item => item.ActualQuantity == 0);
            Console.WriteLine($"Осталось проверить: {uncheckedCount} товаров");
            Console.WriteLine($"Проверено: {activeCheck.CheckedItems.Count - uncheckedCount} товаров");
            Console.WriteLine();

            var uncheckedItems = activeCheck.CheckedItems.Where(item => item.ActualQuantity == 0).ToList();

            if (uncheckedCount == 0)
            {
                Console.WriteLine("Все товары проверены. Завершите инвентаризацию.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Проверка товаров (введите фактическое количество):");
            Console.WriteLine(new string('-', 80));

            int processed = 0;
            foreach (var item in uncheckedItems)
            {
                if (processed >= 10) // За один раз проверяем не более 10 товаров
                {
                    Console.WriteLine($"\nПроверено {processed} товаров за эту сессию.");
                    break;
                }

                Console.WriteLine($"\nТовар: {item.Product.Name}");
                Console.WriteLine($"Место: {item.Product.Location}");
                Console.WriteLine($"Штрих-код: {item.Product.Barcode}");
                Console.WriteLine($"Системное количество: {item.SystemQuantity}");
                Console.Write("Фактическое количество: ");

                if (int.TryParse(Console.ReadLine(), out int actualQuantity))
                {
                    item.ActualQuantity = actualQuantity;
                    processed++;
                }
                else
                {
                    Console.WriteLine("Неверное количество, пропускаем.");
                }
            }

            Console.WriteLine($"\n✓ Проверено товаров в этой сессии: {processed}");
            Console.WriteLine($"Осталось проверить: {uncheckedCount - processed}");
            Console.ReadLine();
        }

        private void CompleteInventoryCheck()
        {
            var activeCheck = inventoryChecks.FirstOrDefault(ic => ic.Status == "В процессе");
            if (activeCheck == null)
            {
                Console.WriteLine("Нет активной инвентаризации.");
                Console.ReadLine();
                return;
            }

            // Проверяем, все ли товары проверены
            int uncheckedCount = activeCheck.CheckedItems.Count(item => item.ActualQuantity == 0);
            if (uncheckedCount > 0)
            {
                Console.WriteLine($"Не все товары проверены! Осталось: {uncheckedCount}");
                Console.Write("Всё равно завершить? (д/н): ");
                if (Console.ReadLine().ToLower() != "д")
                {
                    return;
                }
            }

            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine($"               ЗАВЕРШЕНИЕ ИНВЕНТАРИЗАЦИИ #{activeCheck.Id}");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine();

            // Анализ результатов
            int matchedCount = activeCheck.CheckedItems.Count(item => item.IsMatch);
            int discrepancies = activeCheck.CheckedItems.Count - matchedCount;
            int surplusCount = activeCheck.CheckedItems.Count(item => item.Difference > 0);
            int shortageCount = activeCheck.CheckedItems.Count(item => item.Difference < 0);

            decimal totalSurplusValue = 0;
            decimal totalShortageValue = 0;

            foreach (var item in activeCheck.CheckedItems.Where(item => !item.IsMatch))
            {
                decimal valueDifference = item.Difference * item.Product.UnitPrice;
                if (item.Difference > 0)
                {
                    totalSurplusValue += valueDifference;
                }
                else
                {
                    totalShortageValue += Math.Abs(valueDifference);
                }
            }

            Console.WriteLine("📊 РЕЗУЛЬТАТЫ ИНВЕНТАРИЗАЦИИ:");
            Console.WriteLine();
            Console.WriteLine($"Всего товаров: {activeCheck.CheckedItems.Count}");
            Console.WriteLine($"Совпадает: {matchedCount}");
            Console.WriteLine($"Расхождений: {discrepancies}");
            Console.WriteLine($"Излишки: {surplusCount} товаров на сумму {totalSurplusValue:C}");
            Console.WriteLine($"Недостачи: {shortageCount} товаров на сумму {totalShortageValue:C}");
            Console.WriteLine();

            if (discrepancies > 0)
            {
                Console.WriteLine("Товары с расхождениями:");
                Console.WriteLine(new string('-', 80));
                foreach (var item in activeCheck.CheckedItems.Where(item => !item.IsMatch))
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine();

                Console.Write("Скорректировать системные данные по результатам? (д/н): ");
                if (Console.ReadLine().ToLower() == "д")
                {
                    foreach (var item in activeCheck.CheckedItems)
                    {
                        item.Product.Quantity = item.ActualQuantity;
                    }
                    Console.WriteLine("✓ Данные скорректированы.");
                }
            }

            Console.Write("Введите комментарий к инвентаризации: ");
            activeCheck.Notes = Console.ReadLine();

            activeCheck.Status = "Завершена";
            Console.WriteLine($"\n✓ Инвентаризация #{activeCheck.Id} завершена.");
            Console.ReadLine();
        }

        private void ViewInventoryHistory()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine("                    ИСТОРИЯ ИНВЕНТАРИЗАЦИЙ");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine();

            if (inventoryChecks.Count == 0)
            {
                Console.WriteLine("Инвентаризаций не проводилось.");
            }
            else
            {
                foreach (var check in inventoryChecks.OrderByDescending(ic => ic.CheckDate))
                {
                    int checkedCount = check.CheckedItems.Count(item => item.ActualQuantity > 0);
                    int discrepancies = check.CheckedItems.Count(item => !item.IsMatch);

                    Console.WriteLine($"{check} | Проверено: {checkedCount}/{check.CheckedItems.Count} | Расхождений: {discrepancies}");
                }
            }

            Console.WriteLine("\nНажмите Enter для продолжения...");
            Console.ReadLine();
        }

        private void ViewInventoryDetails()
        {
            Console.Write("Введите ID инвентаризации: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Неверный ID.");
                Console.ReadLine();
                return;
            }

            var check = inventoryChecks.FirstOrDefault(ic => ic.Id == id);
            if (check == null)
            {
                Console.WriteLine("Инвентаризация не найдена.");
                Console.ReadLine();
                return;
            }

            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine($"               ДЕТАЛИ ИНВЕНТАРИЗАЦИИ #{check.Id}");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine();
            Console.WriteLine($"Дата: {check.CheckDate:dd.MM.yyyy HH:mm:ss}");
            Console.WriteLine($"Проверяющий: {check.Inspector}");
            Console.WriteLine($"Статус: {check.Status}");
            Console.WriteLine($"Комментарий: {check.Notes}");
            Console.WriteLine();

            int matchedCount = check.CheckedItems.Count(item => item.IsMatch);
            int discrepancies = check.CheckedItems.Count - matchedCount;

            Console.WriteLine($"Всего товаров: {check.CheckedItems.Count}");
            Console.WriteLine($"Совпадает: {matchedCount}");
            Console.WriteLine($"Расхождений: {discrepancies}");
            Console.WriteLine();

            if (discrepancies > 0)
            {
                Console.WriteLine("Товары с расхождениями:");
                Console.WriteLine(new string('-', 80));
                foreach (var item in check.CheckedItems.Where(item => !item.IsMatch))
                {
                    Console.WriteLine(item);
                }
            }

            Console.WriteLine("\nНажмите Enter для продолжения...");
            Console.ReadLine();
        }

        // Отчёты и аналитика
        private void Reports()
        {
            bool viewing = true;
            while (viewing)
            {
                Console.Clear();
                Console.WriteLine("══════════════════════════════════════════════════════════════════════════");
                Console.WriteLine("                 ОТЧЁТЫ И АНАЛИТИКА");
                Console.WriteLine("══════════════════════════════════════════════════════════════════════════");
                Console.WriteLine();
                Console.WriteLine("1. Отчёт по остаткам товаров");
                Console.WriteLine("2. Отчёт по движению товаров");
                Console.WriteLine("3. Отчёт по поставщикам");
                Console.WriteLine("4. Финансовый отчёт");
                Console.WriteLine("5. Анализ эффективности склада");
                Console.WriteLine("6. Вернуться в главное меню");
                Console.WriteLine();
                Console.Write("Выберите отчёт (1-6): ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        InventoryReport();
                        break;
                    case "2":
                        MovementReport();
                        break;
                    case "3":
                        SupplierReport();
                        break;
                    case "4":
                        FinancialReport();
                        break;
                    case "5":
                        WarehouseEfficiencyReport();
                        break;
                    case "6":
                        viewing = false;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор.");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private void InventoryReport()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine("                    ОТЧЁТ ПО ОСТАТКАМ ТОВАРОВ");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine();

            if (products.Count == 0)
            {
                Console.WriteLine("Товаров нет.");
            }
            else
            {
                decimal totalValue = products.Sum(p => p.UnitPrice * p.Quantity);
                int totalQuantity = products.Sum(p => p.Quantity);

                Console.WriteLine($"Всего товаров: {products.Count} позиций");
                Console.WriteLine($"Общее количество: {totalQuantity} единиц");
                Console.WriteLine($"Общая стоимость запасов: {totalValue:C}");
                Console.WriteLine();

                Console.WriteLine("📊 РАСПРЕДЕЛЕНИЕ ПО КАТЕГОРИЯМ:");
                Console.WriteLine();
                var groupedByCategory = products.GroupBy(p => p.Category);
                foreach (var group in groupedByCategory)
                {
                    decimal categoryValue = group.Sum(p => p.UnitPrice * p.Quantity);
                    int categoryQuantity = group.Sum(p => p.Quantity);
                    double percentage = (double)categoryValue / (double)totalValue * 100;

                    Console.WriteLine($"  {group.Key,-15}: {categoryQuantity,6} ед. | {categoryValue,12:C} | {percentage,5:F1}%");
                }

                Console.WriteLine();
                Console.WriteLine("⚠️  ТОВАРЫ С НИЗКИМ ЗАПАСОМ:");
                Console.WriteLine();
                var lowStock = products.Where(p => p.Quantity < p.MinStockLevel).OrderBy(p => p.Quantity).Take(10);
                if (lowStock.Any())
                {
                    foreach (var product in lowStock)
                    {
                        int needed = product.MinStockLevel - product.Quantity;
                        Console.WriteLine($"  {product.Name,-25} | Остаток: {product.Quantity,4} | Нужно: {needed,4} | Категория: {product.Category}");
                    }
                }
                else
                {
                    Console.WriteLine("  Нет товаров с низким запасом");
                }

                Console.WriteLine();
                Console.WriteLine("📈 ТОП-10 САМЫХ ДОРОГИХ ЗАПАСОВ:");
                Console.WriteLine();
                var topValuable = products.OrderByDescending(p => p.UnitPrice * p.Quantity).Take(10);
                foreach (var product in topValuable)
                {
                    decimal productValue = product.UnitPrice * product.Quantity;
                    Console.WriteLine($"  {product.Name,-25} | {productValue,12:C} | {product.Quantity,4} ед. | {product.UnitPrice,8:C}/ед.");
                }
            }

            Console.WriteLine("\nНажмите Enter для продолжения...");
            Console.ReadLine();
        }

        private void MovementReport()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine("                   ОТЧЁТ ПО ДВИЖЕНИЮ ТОВАРОВ");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine();

            DateTime today = DateTime.Today;
            DateTime weekAgo = today.AddDays(-7);
            DateTime monthAgo = today.AddDays(-30);

            // Приходы
            var recentReceipts = receipts.Where(r => r.ReceiptDate >= monthAgo).ToList();
            var receiptsThisWeek = recentReceipts.Where(r => r.ReceiptDate >= weekAgo).ToList();

            // Расходы
            var recentExpenditures = expenditures.Where(e => e.ExpenditureDate >= monthAgo).ToList();
            var expendituresThisWeek = recentExpenditures.Where(e => e.ExpenditureDate >= weekAgo).ToList();

            Console.WriteLine("📦 ПРИХОД ТОВАРОВ:");
            Console.WriteLine();
            Console.WriteLine($"  За месяц: {recentReceipts.Count} документов на сумму {recentReceipts.Sum(r => r.TotalAmount):C}");
            Console.WriteLine($"  За неделю: {receiptsThisWeek.Count} документов на сумму {receiptsThisWeek.Sum(r => r.TotalAmount):C}");
            Console.WriteLine();

            if (recentReceipts.Count > 0)
            {
                Console.WriteLine("  Топ-5 поставщиков за месяц:");
                var topSuppliers = recentReceipts.GroupBy(r => r.Supplier)
                                                 .Select(g => new { Supplier = g.Key, Count = g.Count(), Amount = g.Sum(r => r.TotalAmount) })
                                                 .OrderByDescending(x => x.Amount)
                                                 .Take(5);
                foreach (var supplier in topSuppliers)
                {
                    Console.WriteLine($"    {supplier.Supplier,-20} | {supplier.Count,3} поставок | {supplier.Amount,12:C}");
                }
            }

            Console.WriteLine();
            Console.WriteLine("📤 РАСХОД ТОВАРОВ:");
            Console.WriteLine();
            Console.WriteLine($"  За месяц: {recentExpenditures.Count} документов на сумму {recentExpenditures.Sum(e => e.TotalAmount):C}");
            Console.WriteLine($"  За неделю: {expendituresThisWeek.Count} документов на сумму {expendituresThisWeek.Sum(e => e.TotalAmount):C}");
            Console.WriteLine();

            if (recentExpenditures.Count > 0)
            {
                Console.WriteLine("  Распределение по назначению:");
                var byPurpose = recentExpenditures.GroupBy(e => e.Purpose)
                                                  .Select(g => new { Purpose = g.Key, Count = g.Count(), Amount = g.Sum(e => e.TotalAmount) });
                foreach (var purpose in byPurpose)
                {
                    Console.WriteLine($"    {purpose.Purpose,-25} | {purpose.Count,3} документов | {purpose.Amount,12:C}");
                }
            }

            Console.WriteLine();
            Console.WriteLine("📈 ОБОРОТ ТОВАРОВ:");
            Console.WriteLine();

            if (recentReceipts.Count > 0 && recentExpenditures.Count > 0)
            {
                decimal totalReceipts = recentReceipts.Sum(r => r.TotalAmount);
                decimal totalExpenditures = recentExpenditures.Sum(e => e.TotalAmount);
                decimal turnover = totalExpenditures > 0 ? totalReceipts / totalExpenditures : 0;

                Console.WriteLine($"  Соотношение приход/расход: {turnover:F2}");
                Console.WriteLine($"  Средний приход: {totalReceipts / recentReceipts.Count:C}");
                Console.WriteLine($"  Средний расход: {totalExpenditures / recentExpenditures.Count:C}");
            }

            Console.WriteLine("\nНажмите Enter для продолжения...");
            Console.ReadLine();
        }

        private void SupplierReport()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine("                    ОТЧЁТ ПО ПОСТАВЩИКАМ");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine();

            if (receipts.Count == 0)
            {
                Console.WriteLine("Нет данных о поставщиках.");
            }
            else
            {
                var supplierData = receipts.GroupBy(r => r.Supplier)
                                          .Select(g => new
                                          {
                                              Supplier = g.Key,
                                              Deliveries = g.Count(),
                                              TotalAmount = g.Sum(r => r.TotalAmount),
                                              LastDelivery = g.Max(r => r.ReceiptDate),
                                              Products = g.SelectMany(r => r.Items.Select(i => i.Product.Name)).Distinct().Count()
                                          })
                                          .OrderByDescending(s => s.TotalAmount)
                                          .ToList();

                Console.WriteLine($"Всего поставщиков: {supplierData.Count}");
                Console.WriteLine();

                Console.WriteLine($"{"Поставщик",-25} | {"Поставки",8} | {"Сумма",12} | {"Товаров",7} | {"Последняя",10}");
                Console.WriteLine(new string('-', 80));

                foreach (var supplier in supplierData)
                {
                    Console.WriteLine($"{supplier.Supplier,-25} | {supplier.Deliveries,8} | {supplier.TotalAmount,12:C} | {supplier.Products,7} | {supplier.LastDelivery:dd.MM.yyyy}");
                }

                Console.WriteLine();
                Console.WriteLine("📊 АНАЛИЗ ПОСТАВЩИКОВ:");
                Console.WriteLine();

                if (supplierData.Count >= 3)
                {
                    Console.WriteLine("  Топ-3 поставщика:");
                    decimal supplierTotal = supplierData.Sum(s => s.TotalAmount);

                    for (int i = 0; i < Math.Min(3, supplierData.Count); i++)
                    {
                        var supplier = supplierData[i];
                        double percentage = (double)supplier.TotalAmount / (double)supplierTotal * 100;
                        Console.WriteLine($"    {i + 1}. {supplier.Supplier,-20} | {percentage,5:F1}% от общего объёма");
                    }
                }

                // Анализ товаров по поставщикам
                Console.WriteLine();
                Console.WriteLine("  Товары по поставщикам:");

                var suppliersWithProducts = products.GroupBy(p => p.Supplier)
                                                   .Where(g => !string.IsNullOrEmpty(g.Key))
                                                   .OrderByDescending(g => g.Sum(p => p.UnitPrice * p.Quantity));

                foreach (var supplierGroup in suppliersWithProducts.Take(5))
                {
                    decimal stockValue = supplierGroup.Sum(p => p.UnitPrice * p.Quantity);
                    Console.WriteLine($"\n    {supplierGroup.Key}:");
                    foreach (var product in supplierGroup.OrderByDescending(p => p.UnitPrice * p.Quantity).Take(3))
                    {
                        Console.WriteLine($"      {product.Name,-20} | {product.Quantity,4} ед. | {product.UnitPrice * product.Quantity,10:C}");
                    }
                    Console.WriteLine($"      Всего на складе: {stockValue,31:C}");
                }
            }

            Console.WriteLine("\nНажмите Enter для продолжения...");
            Console.ReadLine();
        }

        private void FinancialReport()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine("                      ФИНАНСОВЫЙ ОТЧЁТ");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine();

            decimal inventoryValue = products.Sum(p => p.UnitPrice * p.Quantity);
            decimal totalReceipts = receipts.Sum(r => r.TotalAmount);
            decimal totalExpenditures = expenditures.Sum(e => e.TotalAmount);

            Console.WriteLine("💰 ОСНОВНЫЕ ФИНАНСОВЫЕ ПОКАЗАТЕЛИ:");
            Console.WriteLine();
            Console.WriteLine($"  Стоимость товаров на складе:     {inventoryValue,40:C}");
            Console.WriteLine($"  Общий приход товаров:            {totalReceipts,40:C}");
            Console.WriteLine($"  Общий расход товаров:            {totalExpenditures,40:C}");
            Console.WriteLine();

            if (totalReceipts > 0)
            {
                decimal turnoverRatio = totalExpenditures > 0 ? totalReceipts / totalExpenditures : 0;
                Console.WriteLine($"  Коэффициент оборачиваемости:      {turnoverRatio,39:F2}");
            }

            // Анализ по периодам
            Console.WriteLine();
            Console.WriteLine("📅 АНАЛИЗ ПО ПЕРИОДАМ:");
            Console.WriteLine();

            DateTime today = DateTime.Today;
            DateTime[] periods = {
                today.AddDays(-7),   // Неделя
                today.AddDays(-30),  // Месяц
                today.AddDays(-90)   // Квартал
            };

            string[] periodNames = { "Неделя", "Месяц", "Квартал" };

            for (int i = 0; i < periods.Length; i++)
            {
                decimal periodReceipts = receipts.Where(r => r.ReceiptDate >= periods[i]).Sum(r => r.TotalAmount);
                decimal periodExpenditures = expenditures.Where(e => e.ExpenditureDate >= periods[i]).Sum(e => e.TotalAmount);

                Console.WriteLine($"  {periodNames[i]}:");
                Console.WriteLine($"    Приход:  {periodReceipts,36:C}");
                Console.WriteLine($"    Расход:  {periodExpenditures,36:C}");
                if (periodExpenditures > 0)
                {
                    decimal periodRatio = periodReceipts / periodExpenditures;
                    Console.WriteLine($"    Соотношение: {periodRatio,33:F2}");
                }
                Console.WriteLine();
            }

            // Анализ рентабельности
            Console.WriteLine("📈 АНАЛИЗ РЕНТАБЕЛЬНОСТИ:");
            Console.WriteLine();

            if (expenditures.Count > 0)
            {
                var salesExpenditures = expenditures.Where(e => e.Purpose == "Продажа").ToList();
                if (salesExpenditures.Count > 0)
                {
                    decimal totalSales = salesExpenditures.Sum(e => e.TotalAmount);
                    decimal averageMargin = 0.3m; // Предполагаемая наценка 30%
                    decimal estimatedProfit = totalSales * averageMargin;

                    Console.WriteLine($"  Продажи на сумму:             {totalSales,36:C}");
                    Console.WriteLine($"  Предполагаемая прибыль (30%): {estimatedProfit,36:C}");
                    Console.WriteLine($"  Рентабельность продаж:        {(estimatedProfit / totalSales * 100),34:F1}%");
                }
            }

            Console.WriteLine("\nНажмите Enter для продолжения...");
            Console.ReadLine();
        }

        private void WarehouseEfficiencyReport()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine("                АНАЛИЗ ЭФФЕКТИВНОСТИ СКЛАДА");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine();

            // Показатели использования пространства
            int totalLocations = locations.Count;
            int occupiedLocations = locations.Count(l => l.IsOccupied);
            int freeLocations = totalLocations - occupiedLocations;
            double occupancyRate = (double)occupiedLocations / totalLocations * 100;

            Console.WriteLine("📊 ИСПОЛЬЗОВАНИЕ ПРОСТРАНСТВА:");
            Console.WriteLine();
            Console.WriteLine($"  Всего мест: {totalLocations}");
            Console.WriteLine($"  Занято: {occupiedLocations} ({occupancyRate:F1}%)");
            Console.WriteLine($"  Свободно: {freeLocations} ({100 - occupancyRate:F1}%)");
            Console.WriteLine();

            // Распределение по зонам
            Console.WriteLine("  Распределение по зонам:");
            var zoneStats = locations.GroupBy(l => l.Zone)
                                     .Select(g => new
                                     {
                                         Zone = g.Key,
                                         Total = g.Count(),
                                         Occupied = g.Count(l => l.IsOccupied),
                                         OccupancyRate = (double)g.Count(l => l.IsOccupied) / g.Count() * 100
                                     })
                                     .OrderBy(z => z.Zone);

            foreach (var zone in zoneStats)
            {
                Console.WriteLine($"    Зона {zone.Zone}: {zone.Occupied}/{zone.Total} мест ({zone.OccupancyRate:F1}%)");
            }

            // Показатели товарооборота
            Console.WriteLine();
            Console.WriteLine("📦 ПОКАЗАТЕЛИ ТОВАРООБОРОТА:");
            Console.WriteLine();

            if (products.Count > 0)
            {
                decimal totalInventoryValue = products.Sum(p => p.UnitPrice * p.Quantity);
                decimal avgInventoryValue = totalInventoryValue / products.Count;

                Console.WriteLine($"  Общая стоимость запасов: {totalInventoryValue:C}");
                Console.WriteLine($"  Средняя стоимость позиции: {avgInventoryValue:C}");
                Console.WriteLine($"  Количество позиций: {products.Count}");

                // ABC анализ
                Console.WriteLine();
                Console.WriteLine("  ABC-анализ запасов:");
                var sortedProducts = products.OrderByDescending(p => p.UnitPrice * p.Quantity).ToList();
                decimal cumulativeValue = 0;
                decimal totalValue = sortedProducts.Sum(p => p.UnitPrice * p.Quantity);

                int aCount = 0, bCount = 0, cCount = 0;
                decimal aValue = 0, bValue = 0, cValue = 0;

                foreach (var product in sortedProducts)
                {
                    decimal productValue = product.UnitPrice * product.Quantity;
                    cumulativeValue += productValue;
                    double percentage = (double)cumulativeValue / (double)totalValue * 100;

                    if (percentage <= 80)
                    {
                        aCount++;
                        aValue += productValue;
                    }
                    else if (percentage <= 95)
                    {
                        bCount++;
                        bValue += productValue;
                    }
                    else
                    {
                        cCount++;
                        cValue += productValue;
                    }
                }

                Console.WriteLine($"    Категория A ({aCount} товаров): {aValue:C} ({(double)aValue / (double)totalValue * 100:F1}%)");
                Console.WriteLine($"    Категория B ({bCount} товаров): {bValue:C} ({(double)bValue / (double)totalValue * 100:F1}%)");
                Console.WriteLine($"    Категория C ({cCount} товаров): {cValue:C} ({(double)cValue / (double)totalValue * 100:F1}%)");
            }

            // Эффективность инвентаризации
            Console.WriteLine();
            Console.WriteLine("📋 ЭФФЕКТИВНОСТЬ ИНВЕНТАРИЗАЦИИ:");
            Console.WriteLine();

            if (inventoryChecks.Count > 0)
            {
                var completedChecks = inventoryChecks.Where(ic => ic.Status == "Завершена").ToList();
                if (completedChecks.Count > 0)
                {
                    var lastCheck = completedChecks.OrderByDescending(ic => ic.CheckDate).First();
                    int discrepancies = lastCheck.CheckedItems.Count(item => !item.IsMatch);
                    double accuracyRate = (double)(lastCheck.CheckedItems.Count - discrepancies) / lastCheck.CheckedItems.Count * 100;

                    Console.WriteLine($"  Последняя инвентаризация: {lastCheck.CheckDate:dd.MM.yyyy}");
                    Console.WriteLine($"  Точность учета: {accuracyRate:F1}%");
                    Console.WriteLine($"  Расхождения: {discrepancies} из {lastCheck.CheckedItems.Count}");
                }
            }

            // Рекомендации
            Console.WriteLine();
            Console.WriteLine("💡 РЕКОМЕНДАЦИИ:");
            Console.WriteLine();

            var lowStockProducts = products.Where(p => p.Quantity < p.MinStockLevel).ToList();
            if (lowStockProducts.Count > 0)
            {
                Console.WriteLine($"  ⚠️  Необходимо пополнить {lowStockProducts.Count} товаров:");
                foreach (var product in lowStockProducts.Take(3))
                {
                    int needed = product.MinStockLevel - product.Quantity;
                    Console.WriteLine($"    - {product.Name}: нужно {needed} ед.");
                }
                if (lowStockProducts.Count > 3)
                {
                    Console.WriteLine($"    ... и ещё {lowStockProducts.Count - 3} товаров");
                }
            }

            if (occupancyRate > 90)
            {
                Console.WriteLine("  ⚠️  Склад почти заполнен! Рассмотрите возможность расширения.");
            }
            else if (occupancyRate < 50)
            {
                Console.WriteLine("  ℹ️  Склад используется менее чем на 50%. Можно разместить больше товаров.");
            }

            Console.WriteLine("\nНажмите Enter для продолжения...");
            Console.ReadLine();
        }
    }

    // Точка входа программы
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var system = new WarehouseManagementSystem();
            system.Run();
        }
    }
}