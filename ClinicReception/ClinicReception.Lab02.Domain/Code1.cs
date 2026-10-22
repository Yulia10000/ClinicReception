using System;
using System.Collections.Generic;
using System.Linq;

namespace ClinicReception.Lab02.Domain
{
    // клас
    public class zayavka
    {
        public string id;
        public string pac;
        public List<string[]> rob = new List<string[]>();
        public int st = 0; // 0-нова,1-підтверджена,2-завершена,3-скасована
        public DateTime dt;
        // public string prim; // примітка, поки не треба

        // конструктор
        public zayavka(string a, string b)
        {
            id = a;              // ставимо id
            pac = b;         // ставимо pac
            dt = DateTime.Now;   // ставимо дату
        }

        // метод додавання
        public void Add(string s, int k, decimal c)
        {
            string[] t = new string[3];
            t[0] = s; t[1] = k.ToString(); t[2] = c.ToString();
            rob.Add(t);   // додаємо t у rob
        }

        // ProcessData
        public decimal ProcessData(bool f)
        {
            decimal sum1 = 0; int kolvo = 0;
            for (int i = 0; i < rob.Count; i++)
            {
                int k = int.Parse(rob[i][1]);
                decimal c = decimal.Parse(rob[i][2]);
                sum1 = sum1 + k * c;   // додаємо до суми
                kolvo = kolvo + 1;     // збільшуємо kolvo на одиницю
            }
            // if (sum1 > 500) { sum1 = sum1 - 50; } // стара знижка
            if (f == true && sum1 > 1000) { sum1 = sum1 * 0.9m; }
            else if (sum1 > 5000) { sum1 = sum1 * 0.85m; }
            else { sum1 = sum1; }
            if (kolvo > 10) sum1 = sum1 - 100;
            if (sum1 < 0) sum1 = 0;
            sum1 = sum1 + sum1 * 0.2m;
            return Math.Round(sum1, 2);   // повертаємо sum1
        }

        // міняємо статус
        public bool Chg(int n)
        {
            if (st == 0 && n == 1) { st = 1; return true; }
            if (st == 1 && n == 2) { st = 2; return true; }
            if (st == 0 && n == 3) { st = 3; return true; }
            return false;   // не можна
        }

        // перевірка
        public bool Ck()
        {
            if (id != null && id != "" && pac != null && pac.Length > 2 && rob.Count > 0 && rob.Count < 100 && st >= 0 && st <= 3)
                return true;
            else
                return false;
        }

        // звіт
        public string Rep()
        {
            string s = "";
            for (int i = 0; i < rob.Count; i++)
            {
                s = s + "Послуга: " + rob[i][0] + "; кількість: " + rob[i][1] + "; ціна: " + rob[i][2] + "; сума: " + (int.Parse(rob[i][1]) * decimal.Parse(rob[i][2])) + "\n";
            }
            s = s + "Разом: " + ProcessData(false) + "\n";
            return s;   // повертаємо s
        }

        // пошук
        public static zayavka F(List<zayavka> L, string x)
        {
            for (int i = 0; i < L.Count; i++)
            {
                if (L[i].id == x) { return L[i]; }
            }
            return null;
        }
    }
}

