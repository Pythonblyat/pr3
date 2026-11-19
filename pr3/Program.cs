using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PraktInterfaces
{
    internal static class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            RunTask1_1();
            Console.WriteLine(new string('=', 60));
            RunTask1_2();
            Console.WriteLine(new string('=', 60));
            RunTask2_1();
            Console.WriteLine(new string('=', 60));
            RunTask3_1();
        }

        private static void RunTask1_1()
        {
            Console.WriteLine("=== ЗАВДАННЯ 1.1: Неявна імплементація IShape та IResizable ===");

            IShape[] shapes = new IShape[]
            {
                new Circle(5.0),
                new Rectangle(3.0, 4.0),
                new Triangle(3, 4, 5),
                new Circle(2.5),
                new Rectangle(6.0, 2.0)
            };

            Console.WriteLine("--- Поліморфізм через IShape ---");
            double totalArea = 0;
            foreach (IShape shape in shapes)
            {
                shape.DisplayInfo();
                totalArea += shape.GetArea();
            }
            Console.WriteLine($"\nЗагальна площа всіх фігур: {totalArea:F2}");

            Console.WriteLine("\n--- Масштабування (IResizable) ---");
            foreach (IShape shape in shapes)
            {
                if (shape is IResizable resizable)
                {
                    Console.Write("До: ");
                    shape.DisplayInfo();
                    resizable.Resize(2.0);
                    Console.Write("Після (x2): ");
                    shape.DisplayInfo();
                }
                else
                {
                    Console.WriteLine($"{shape.Name} — не підтримує масштабування");
                }
            }
        }

        private static void RunTask1_2()
        {
            Console.WriteLine("=== ЗАВДАННЯ 1.2: Явна імплементація інтерфейсу ===");

            Report report = new Report("Фінансовий звіт",
                "Дохід за квартал: 150 000 грн. Витрати: 95 000 грн.");

            report.ShowSummary();

            IScreenPrintable screen = report;
            Console.WriteLine($"\nФормат екрану: {screen.GetFormat()}");
            screen.Print();

            IPrinterPrintable printer = report;
            Console.WriteLine($"\nФормат принтера: {printer.GetFormat()}");
            printer.Print();

            Console.WriteLine("\n=== Через приведення типу ===");
            ((IScreenPrintable)report).Print();
            ((IPrinterPrintable)report).Print();

            // report.Print(); // Неможливо: явно реалізовані члени недоступні через тип класу Report (помилка компіляції)
            Console.WriteLine("Примітка: виклик report.Print() напряму неможливий через явну імплементацію.");
        
        }
        private static void RunTask2_1()
        {
            Console.WriteLine("=== ЗАВДАННЯ 2.1: Множинна реалізація інтерфейсів ===");

            User user1 = new User("admin", "admin@example.com", 25);
            User user2 = new User("", "invalid-email", 15);
            Order order = new Order(1, "Ноутбук", 25000m);

            Console.WriteLine("--- Валідація (IValidatable) ---");
            IValidatable[] validatables = { user1, user2, order };
            foreach (var v in validatables)
            {
                Console.WriteLine($"{v}: Valid = {v.IsValid()}");
                if (!v.IsValid())
                {
                    foreach (string err in v.GetErrors())
                        Console.WriteLine($"  Помилка: {err}");
                }
            }

            Console.WriteLine("\n--- Серіалізація (ISerializable) ---");
            ISerializable[] serializables = { user1, order };
            foreach (var s in serializables)
            {
                Console.WriteLine(s.Serialize());
            }

            Console.WriteLine("\n--- Збереження (IPersistable) ---");
            IPersistable[] persistables = { user1, user2 };
            foreach (var p in persistables)
            {
                p.Save();
            }

            Console.WriteLine("\n--- Історія логів (ILoggable) ---");
            user1.Log("Спроба збереження");
            foreach (string log in user1.GetLogHistory())
                Console.WriteLine($"  {log}");
        }
        private static void RunTask3_1()
        {
            Console.WriteLine("=== ЗАВДАННЯ 3.1: IComparable<T> та IEnumerable<T> ===");

            StudentGroup group = new StudentGroup("КІ-21");
            group.AddStudent(new Student("Олена Петренко", "КІ-21", 4.8));
            group.AddStudent(new Student("Андрій Коваленко", "КІ-21", 3.5));
            group.AddStudent(new Student("Ірина Мельник", "КІ-21", 4.9));
            group.AddStudent(new Student("Максим Бондаренко", "КІ-21", 4.2));
            group.AddStudent(new Student("Наталія Шевченко", "КІ-21", 4.6));
            group.AddStudent(new Student("Дмитро Ткаченко", "КІ-21", 3.9));

            Console.WriteLine("--- До сортування ---");
            group.DisplayAll();

            Console.WriteLine("\n--- Після сортування за GPA ---");
            group.SortByGPA();
            group.DisplayAll();

            Console.WriteLine("\n--- Відмінники (GPA >= 4.5) ---");
            foreach (Student s in group.GetHonorStudents())
                Console.WriteLine($"  {s}");

            Console.WriteLine($"\nСередній GPA: {group.GetAverageGPA():F2}");
            Console.WriteLine($"Найкращий студент: {group.GetTopStudent()}");

            Console.WriteLine("\n--- LINQ запити ---");
            var top3 = group.OrderByDescending(s => s.GPA).Take(3);
            Console.WriteLine("Топ-3 за GPA:");
            foreach (var s in top3)
                Console.WriteLine($"  {s}");

            var averageAbove4 = group.Where(s => s.GPA > 4.0).Average(s => s.GPA);
            Console.WriteLine($"Середній GPA серед тих, хто > 4.0: {averageAbove4:F2}");
        }
    }

    // ================= ЗАВДАННЯ 1.1 =================
    public interface IShape
    {
        string Name { get; }
        double GetArea();
        double GetPerimeter();
        void DisplayInfo();
    }

    public interface IResizable
    {
        void Resize(double factor);
    }

    public class Circle : IShape, IResizable
    {
        private double _radius;

        public Circle(double radius)
        {
            if (radius <= 0)
                throw new ArgumentOutOfRangeException(nameof(radius), "Радіус має бути додатним");
            _radius = radius;
        }

        public string Name => "Коло";
        public double GetArea() => Math.PI * _radius * _radius;
        public double GetPerimeter() => 2 * Math.PI * _radius;

        public void DisplayInfo()
        {
            Console.WriteLine($"{Name}: R = {_radius:F2}, Площа = {GetArea():F2}, Периметр = {GetPerimeter():F2}");
        }

        public void Resize(double factor)
        {
            if (factor <= 0)
                throw new ArgumentOutOfRangeException(nameof(factor), "Коефіцієнт масштабування має бути додатним");
            _radius *= factor;
        }
    }

    public class Rectangle : IShape, IResizable
    {
        private double _width;
        private double _height;

        public Rectangle(double width, double height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("Сторони прямокутника мають бути додатними");
            _width = width;
            _height = height;
        }

        public string Name => "Прямокутник";
        public double GetArea() => _width * _height;
        public double GetPerimeter() => 2 * (_width + _height);

        public void DisplayInfo()
        {
            Console.WriteLine($"{Name}: {_width:F2} x {_height:F2}, Площа = {GetArea():F2}, Периметр = {GetPerimeter():F2}");
        }

        public void Resize(double factor)
        {
            if (factor <= 0)
                throw new ArgumentOutOfRangeException(nameof(factor), "Коефіцієнт масштабування має бути додатним");
            _width *= factor;
            _height *= factor;
        }
    }

    public class Triangle : IShape
    {
        private readonly double _a, _b, _c;

        public Triangle(double a, double b, double c)
        {
            if (a <= 0 || b <= 0 || c <= 0 || a + b <= c || a + c <= b || b + c <= a)
                throw new ArgumentException("Сторони не утворюють трикутник");
            _a = a;
            _b = b;
            _c = c;
        }

        public string Name => "Трикутник";

        public double GetArea()
        {
            double s = (_a + _b + _c) / 2.0;
            return Math.Sqrt(s * (s - _a) * (s - _b) * (s - _c));
        }

        public double GetPerimeter() => _a + _b + _c;

        public void DisplayInfo()
        {
            Console.WriteLine($"{Name}: сторони {_a:F2}, {_b:F2}, {_c:F2}, Площа = {GetArea():F2}, Периметр = {GetPerimeter():F2}");
        }
    }

    // ================= ЗАВДАННЯ 1.2 =================
    public interface IScreenPrintable
    {
        void Print();
        string GetFormat();
    }

    public interface IPrinterPrintable
    {
        void Print();
        string GetFormat();
    }

    public class Report : IScreenPrintable, IPrinterPrintable
    {
        public string Title { get; set; }
        public string Content { get; set; }

        public Report(string title, string content)
        {
            Title = title;
            Content = content;
        }

        void IScreenPrintable.Print()
        {
            Console.WriteLine($"[ЕКРАН] Звіт: {Title}\n{Content}");
        }

        void IPrinterPrintable.Print()
        {
            Console.WriteLine($"[ПРИНТЕР] Друк звіту: {Title} ({Content.Length} символів)");
        }

        string IScreenPrintable.GetFormat() => "HTML";
        string IPrinterPrintable.GetFormat() => "PDF";

        public void ShowSummary()
        {
            Console.WriteLine($"Звіт: {Title} ({Content.Length} символів)");
        }
    }
    // ================= ЗАВДАННЯ 2.1 =================
    public interface ISerializable
    {
        string Serialize();
    }

    public interface IValidatable
    {
        bool IsValid();
        List<string> GetErrors();
    }

    public interface ILoggable
    {
        void Log(string action);
        List<string> GetLogHistory();
    }

    public interface IPersistable : ISerializable, IValidatable
    {
        void Save();
        void Load(string data);
    }

    public class User : IPersistable, ILoggable
    {
        private readonly List<string> _logHistory = new List<string>();

        public string Username { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }

        public User(string username, string email, int age)
        {
            Username = username;
            Email = email;
            Age = age;
            Log("Створено користувача");
        }

        public string Serialize() =>
            $"{{\"username\":\"{Username}\", \"email\":\"{Email}\", \"age\":{Age}}}";

        public bool IsValid() => GetErrors().Count == 0;

        public List<string> GetErrors()
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(Username))
                errors.Add("Ім'я користувача не може бути порожнім");
            if (string.IsNullOrWhiteSpace(Email) || !Email.Contains('@'))
                errors.Add("Email має містити символ '@'");
            if (Age < 18)
                errors.Add("Вік має бути не менше 18 років");
            return errors;
        }

        public void Save()
        {
            if (IsValid())
            {
                string json = Serialize();
                Log($"Збережено користувача {Username}");
                Console.WriteLine($"Збережено: {json}");
            }
            else
            {
                Log($"Помилка збереження користувача '{Username}'");
                Console.WriteLine($"Неможливо зберегти користувача '{Username}':");
                foreach (var err in GetErrors())
                    Console.WriteLine($"  - {err}");
            }
        }

        public void Load(string data)
        {
            if (!string.IsNullOrWhiteSpace(data))
            {
                Username = data.Trim();
                Log($"Завантажено дані: {Username}");
            }
        }

        public void Log(string action)
        {
            _logHistory.Add($"[{DateTime.Now:HH:mm:ss}] {action}");
        }

        public List<string> GetLogHistory() => new List<string>(_logHistory);

        public override string ToString() => $"User: {Username}, Email: {Email}, Age: {Age}";
    }

    public class Order : IPersistable, ILoggable
    {
        private readonly List<string> _logHistory = new List<string>();

        public int OrderId { get; set; }
        public string Product { get; set; }
        public decimal Amount { get; set; }

        public Order(int orderId, string product, decimal amount)
        {
            OrderId = orderId;
            Product = product;
            Amount = amount;
            Log($"Створено замовлення #{OrderId}");
        }

        public string Serialize() =>
            $"{{\"orderId\":{OrderId}, \"product\":\"{Product}\", \"amount\":{Amount}}}";

        public bool IsValid() => GetErrors().Count == 0;

        public List<string> GetErrors()
        {
            var errors = new List<string>();
            if (OrderId <= 0)
                errors.Add("OrderId має бути більшим за 0");
            if (string.IsNullOrWhiteSpace(Product))
                errors.Add("Назва товару не може бути порожньою");
            if (Amount <= 0)
                errors.Add("Сума замовлення має бути більшою за 0");
            return errors;
        }

        public void Save()
        {
            if (IsValid())
            {
                Log($"Збережено замовлення #{OrderId}");
                Console.WriteLine($"Збережено: {Serialize()}");
            }
            else
            {
                Console.WriteLine($"Помилки замовлення #{OrderId}: {string.Join("; ", GetErrors())}");
            }
        }

        public void Load(string data)
        {
            if (!string.IsNullOrWhiteSpace(data))
            {
                Product = data.Trim();
                Log($"Оновлено товар замовлення #{OrderId}");
            }
        }

        public void Log(string action)
        {
            _logHistory.Add($"[{DateTime.Now:HH:mm:ss}] {action}");
        }

        public List<string> GetLogHistory() => new List<string>(_logHistory);

        public override string ToString() => $"Order #{OrderId}: {Product}, {Amount:F2} грн";
    }
    // ================= ЗАВДАННЯ 3.1 =================
    public class Student : IComparable<Student>
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public double GPA { get; set; }

        public Student(string name, string group, double gpa)
        {
            Name = name;
            Group = group;
            GPA = gpa;
        }

        // Спадний порядок: від найвищого GPA до найнижчого
        public int CompareTo(Student? other)
        {
            if (other == null) return -1;
            return other.GPA.CompareTo(GPA);
        }

        public override string ToString() => $"{Name} ({Group}), GPA: {GPA:F2}";
    }

    public class StudentGroup : IEnumerable<Student>
    {
        private readonly List<Student> _students = new List<Student>();
        public string GroupName { get; set; }

        public StudentGroup(string groupName)
        {
            GroupName = groupName;
        }

        public void AddStudent(Student student) => _students.Add(student);
        public int Count => _students.Count;
        public Student this[int index] => _students[index];

        public IEnumerable<Student> GetHonorStudents(double threshold = 4.5)
        {
            foreach (var student in _students)
            {
                if (student.GPA >= threshold)
                    yield return student;
            }
        }

        public IEnumerator<Student> GetEnumerator() => _students.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void SortByGPA() => _students.Sort();

        public double GetAverageGPA()
        {
            if (_students.Count == 0) return 0;
            return _students.Average(s => s.GPA);
        }

        public Student? GetTopStudent()
        {
            if (_students.Count == 0) return null;
            Student top = _students[0];
            foreach (var s in _students)
            {
                if (s.GPA > top.GPA)
                    top = s;
            }
            return top;
        }

        public void DisplayAll()
        {
            Console.WriteLine($"=== Група {GroupName} ({Count} студентів) ===");
            foreach (Student s in this)
                Console.WriteLine($"  {s}");
        }
    }
}