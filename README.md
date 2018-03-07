# Enki.WorkTime

**Portuguese:**
Documentação do componente de processamento de horas úteis.

**English:**
Documentation about lib to calc business days.

## Sumário / Summary

1. Conceitos
2. Build

## 1. Conceitos / Concepts

1. Hora útil(WorkTime) é considerado um período de trabalho pré-configurado, por exemplo Segunda a Sexta das 08:00 as 16:00
2. Feriado(Exception) é considerado o período no dia de trabalho que não será trabalhado.
    1. Por exemplo Numa segunda-feira onde o horário de trabalho é das 08:00 as 16:00, podemos configurar um feriado para o mesmo dia das 00:00 as 12:00, e no caso nesta data serão consideradas apenas 4 horas de trabalho.
3. Feriados recorrentes(RecurrentException) é considerado um feriado que vale na mesma data, independentemente do ano.

## 2. Situação Build / Build Situation

[![Build status](https://ci.appveyor.com/api/projects/status/h9mk1uyuhk635cbs?svg=true)](https://ci.appveyor.com/project/reinaldocoelho/enki-worktime)