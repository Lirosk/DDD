import tkinter as tk
import pyperclip
import pytesseract
from PIL import ImageGrab, ImageTk, Image

pytesseract.pytesseract.tesseract_cmd = r'C:\Program Files\Tesseract-OCR\tesseract.exe'

# Функция для обработки нажатия Ctrl+V
def handle_paste(event):
    # Получение изображения из буфера обмена
    image = ImageGrab.grabclipboard()

    # Если изображение существует и является изображением
    if image and isinstance(image, Image.Image):
        # Определение размеров окна и изображения
        text = pytesseract.image_to_string(image, lang='rus+eng', config=r'--oem 3 --psm 6')

        window_width = 400
        window_height = 300
        image_width, image_height = image.size

        # Масштабирование изображения в соответствии с размерами окна
        scale = min(window_width / image_width, window_height / image_height)
        new_width = int(image_width * scale)
        new_height = int(image_height * scale)
        resized_image = image.resize((new_width, new_height), Image.ANTIALIAS)

        # Отображение изображения в окне
        photo = ImageTk.PhotoImage(resized_image)
        image_label.configure(image=photo)
        image_label.image = photo

        text_entry.delete('1.0', tk.END)
        text_entry.insert(tk.END, text)

# Функция для копирования текста в буфер обмена
def copy_text():
    text = text_entry.get('1.0', tk.END).strip()
    if text:
        pyperclip.copy(text)

# Создание главного окна
window = tk.Tk()
window.title("Распознавание текста изображения")
window.geometry("600x600")

# Создание метки для изображения
image_label = tk.Label(window)
image_label.pack()

# Создание метки и поля для распознанного текста
text_label = tk.Label(window, text="Распознанный текст:")
text_label.pack()

# Создание горизонтального скроллера для поля с текстом
scrollbar = tk.Scrollbar(window, orient="horizontal")
scrollbar.pack(fill="x")

text_entry = tk.Text(window, height=20, width=80, wrap="none", xscrollcommand=scrollbar.set)
text_entry.pack()

scrollbar.config(command=text_entry.xview)

# Создание кнопки копирования текста
copy_button = tk.Button(window, text="Копировать", command=copy_text)
copy_button.pack()

# Привязка обработчика события для нажатия Ctrl+V
window.bind("<Control-v>", handle_paste)

# Запуск главного цикла окна
window.mainloop()
