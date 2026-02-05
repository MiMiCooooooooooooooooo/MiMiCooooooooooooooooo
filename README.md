# Money Manager (.NET 8 Web API)

Bu loyiha rasmda berilgan texnik topshiriq asosida tayyorlangan minimal backend prototipidir.

## Qamrab olingan funksiyalar

1. **Ro'yxatdan o'tish / kirish**
   - Email + parol bilan ro'yxatdan o'tish
   - OTP tekshiruv (demo rejimida kod javobda qaytadi)
   - JWT orqali autentifikatsiya
2. **Daromad / xarajat qo'shish**
   - Majburiy maydonlar: categoryId, amount, type
   - Qo'shimcha maydon: note
   - Sana avtomatik `UTC now`
3. **Kategoriya boshqaruvi**
   - Tasdiqlangandan keyin default kategoriyalar avtomatik qo'shiladi
   - Foydalanuvchi custom kategoriya qo'sha oladi
4. **Byudjet belgilash**
   - Kategoriya bo'yicha oy/yil kesimida limit
   - Xarajat limitdan oshsa javobda `budgetExceeded=true`
5. **Hisobotlar**
   - Weekly / Monthly / Yearly endpointlari
   - Umumiy daromad, xarajat, balans, savings-rate va kategoriya kesimi
6. **Yuklab olish**
   - `.xlsx` formatida tranzaksiyalar eksporti
7. **Bot integratsiya uchun tayyor endpoint**
   - Gmail webhook stub endpoint

## Ishga tushirish

```bash
cd MoneyManager.Api
dotnet restore
dotnet run
```

Swagger: `https://localhost:xxxx/swagger`

## Asosiy endpointlar

- `POST /api/auth/register`
- `POST /api/auth/verify-otp`
- `POST /api/auth/login`
- `GET /api/categories`
- `POST /api/categories`
- `POST /api/transactions`
- `POST /api/budgets`
- `GET /api/reports/weekly`
- `GET /api/reports/monthly`
- `GET /api/reports/yearly`
- `GET /api/export/xlsx`
- `POST /api/bot/gmail-webhook`

## Eslatma

Bu versiya **in-memory** saqlashdan foydalanadi. Production uchun:
- EF Core + PostgreSQL/MSSQL
- OTP uchun SMS/Email provayder
- Budget alert uchun background jobs (email/push)
- Frontend (React/Blazor/Mobile) qo'shish tavsiya etiladi.
