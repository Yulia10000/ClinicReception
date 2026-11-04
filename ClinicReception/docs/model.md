
# Класова модель ClinicReception


## 1. Сутності та їхні інваріанти

| Сутність | Роль | Незмінність | Інваріант |
|---|---|---|---|
| Appointment | Запис пацієнта на прийом | Ні, змінний агрегат | Id > 0, PatientId > 0, DoctorId > 0 |
| VisitItem | Послуга в записі | Так, sealed record | Quantity > 0, UnitPrice >= 0 |
| Patient | Довідник пацієнтів | Так, sealed record | Id > 0, ім'я не порожнє, Email містить @ |
| MedicalService | Довідник медичних послуг | Так, sealed record | Id > 0, назва не порожня, Price >= 0 |
| Doctor | Довідник лікарів | Так, sealed record | Id > 0, ім'я та спеціальність не порожні |

## 2. Стани та дозволені переходи між ними

| Зі стану | До стану | Умова |
|---|---|---|
| New | Confirmed | Заповнені дані пацієнта та лікаря |
| New | Cancelled | Скасування запису |
| Confirmed | Completed | Прийом відбувся |
| Confirmed | Rescheduled | Визначено нову дату прийому |
| Confirmed | Cancelled | Запис скасовано |
| Rescheduled | Confirmed | Нова дата прийому погоджена |
| Rescheduled | Cancelled | Запис скасовано |



