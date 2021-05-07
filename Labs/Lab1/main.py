import os
import numpy as np

def check (name):
    if (os.path.exists(name)):
        print('Файл успешно сохранен.')
    else:
        print('ОШИБКА! Не удалось сохранить файл.')
        return 0

def ABC_RU():
    a = 'АаБбВвГгДдЕеЁёЖжЗзИиЙйКкЛлМмНнОоПпРрСсТтУуФфХхЦцЧчШшЩщЪъЫыЬьЭэЮюЯя'
    b = '1234567890@#–—$_&-+()/*":;!?,.~`|•√π÷×¶∆£¢€¥^°={}\%©®™✓[]<> '
    return a, b

def ABC_EN():
    a = ['A', 'B', 'C', 'D',
         'E', 'F', 'G', 'H',
         'I', 'J', 'K', 'L',
         'M', 'N', 'O', 'P',
         'Q', 'R', 'S', 'T',
         'U', 'V', 'W', 'X',
         'Y', 'Z']
    return a

def simple_choice():
    flag = True
    while (flag == True):
        print('Хотите продолжить?\n'
              '1) Да\n'
              '2) Нет')
        choice = int(input())
        if (choice == 1):
            flag = False
            menu()
        elif (choice == 2):
            exit(0)
        else:
            print('Выбрана неверная операция! Повторите попытку!\n')

def menu ():
    flag = True
    while (flag == True):
        print('Выберете нужную операцию:\n'
              '1) Зашивровать текст на русском языке с помощью шифра Цезаря\n'
              '2) Дешифровать текст на русском с помощью шифра Цезаря\n'
              '3) Зашифравать текст на английском с помощью шифра Виженера\n'
              '4) Дешифровать текст на английском с помощью шифра Виженера\n'
              '5) Выйти из программы')
        choice = int(input())
        if (choice == 1):
            flag = False
            task1(choice)
        elif (choice == 2):
            task1(choice)
            flag = False
        elif (choice == 3):
            task2(choice)
            flag = False
        elif (choice == 4):
            task2(choice)
            flag = False
        elif (choice == 5):
            exit(0)
        else:
            print('Выбрана неверная операция! Повторите попытку!\n')

def read_file ():
    input_file = input('Введите путь для сохранения файла с оригинальным текстом: ')
    f = open(input_file, 'w')
    f.write(input('Введите текст: '))
    f.close()
    check(input_file)
    with open(input_file) as f:
        shifr = f.read()
    f.close()
    return shifr

def Caesar (text, number, a, b):
    output_file = input('Введите путь для сохранения файла с зашифрованным текстом: ')
    f = open(output_file, 'w')
    for i in range(len(text)):
        for j in range(len(a)):
            if (text[i] == a[j]):
                j += 2 * number
                if (j > 64):
                    j -= 66
                    f.write(a[j] + '')
                else:
                    f.write(a[j] + '')
        if (text[i] in b):
                f.write(text[i] + '')
    f.close()
    check(output_file)

def Caesar2 (text, number, a, b):
    output_file = input('Введите путь для сохранения файла с дешифрованным текстом: ')
    f = open(output_file, 'w')
    for i in range(len(text)):
        for j in range(len(a)):
            if (text[i] == a[j]):
                j -= 2 * number
                if (j > 64):
                    j = j - 66
                    f.write(a[j] + '')
                else:
                    f.write(a[j] + '')
        else:
            if (text[i] in b):
                f.write(text[i] + '')
    f.close()
    check(output_file)

def Vigener (text, key, choice, a):
    if (choice == 3):
        output_file = input('Введите путь для сохранения файла с зашифрованным текстом: ')
    elif (choice == 4):
        output_file = input('Введите путь для сохранения файла с дешифрованным текстом: ')
    f = open(output_file, 'w')
    shifr = np.array([0 for i in range(len(text))])
    result = np.array(['' for i in range(len(text))])
    iter, c1, c2 = 0, 0, 0
    for i in range(len(text)):
        if (iter >= len(key)):
            iter = 0
        for j in range(len(a)):
            if (text[i] == a[j]):
                c1 = j
            if (key[iter] == a[j]):
                c2 = j
        if (choice == 3):
            shifr[i] = (c1 + c2) % 26
        elif (choice == 4):
            shifr[i] = (c1 - c2 + 26) % 26
        for j in range(len(a)):
            if (shifr[i] == j):
                result[i] = a[j]
                f.write(result[i] + '')
        iter += 1
    f.close()
    check(output_file)

def task1 (choice):
    a, b = ABC_RU()
    text = read_file()
    number = int(input('Введите ключ: '))
    if (choice == 1):
        Caesar(text, number, a, b)
    elif (choice == 2):
        Caesar2(text, number, a, b)
    simple_choice()

def task2 (choice):
    a = ABC_EN()
    text = read_file()
    key = input('Введите ключ: ')
    Vigener(text, key, choice, a)
    simple_choice()
if __name__ == '__main__':
    menu()