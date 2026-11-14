# Comparacion: busqueda lineal contra binaria

- Lenguaje: C#
- Elementos: 1.000.000
- Consultas: 1.000
- Protocolo: 1 calentamiento + 3 corridas + promedio

| Corrida | Lineal (ms) | Binaria (ms) |
|---|---:|---:|
| 1 | 223.787 | 0.543 |
| 2 | 197.020 | 0.580 |
| 3 | 199.784 | 0.210 |

| **Promedio** | **206.864** | **0.444** |

**Cociente:** 465.6 veces.

## Conclusion sobre mi proceso H1

Conviene ordenar una sola vez los registros cuando se realizaran muchas consultas sobre los mismos datos, porque la busqueda binaria permite encontrar informacion mas rapidamente.

Si se realizan pocas consultas o los datos cambian constantemente, puede convenir la busqueda lineal para evitar el costo de ordenar primero.
