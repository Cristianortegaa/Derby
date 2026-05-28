# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: arbitro.spec.ts >> Panel del Árbitro >> en el historial puede abrir y cerrar el modal de detalles
- Location: tests\arbitro.spec.ts:74:7

# Error details

```
Tearing down "context" exceeded the test timeout of 30000ms.
```

# Page snapshot

```yaml
- main [ref=e4]:
  - generic [ref=e6]:
    - navigation [ref=e7]:
      - generic [ref=e9]:
        - generic [ref=e10] [cursor=pointer]:
          - img "Liga Derby" [ref=e12]
          - generic [ref=e14]: LIGA DERBY
        - generic [ref=e15]:
          - link " Inicio" [ref=e16] [cursor=pointer]:
            - /url: /
            - generic [ref=e17]: 
            - generic [ref=e18]: Inicio
          - link " Competiciones" [ref=e19] [cursor=pointer]:
            - /url: /competiciones
            - generic [ref=e20]: 
            - generic [ref=e21]: Competiciones
          - link " Clubes" [ref=e22] [cursor=pointer]:
            - /url: /clubes
            - generic [ref=e23]: 
            - generic [ref=e24]: Clubes
        - button " arbitro1@derby.com " [ref=e26] [cursor=pointer]:
          - generic [ref=e27]:
            - generic [ref=e28]: 
            - generic [ref=e29]: arbitro1@derby.com
          - generic [ref=e30]: 
    - generic [ref=e33]:
      - generic [ref=e34]:
        - heading "Historial de Partidos" [level=1] [ref=e35]
        - paragraph [ref=e36]: Partidos arbitrados histórico
      - button " Volver" [ref=e38] [cursor=pointer]:
        - generic [ref=e39]: 
        - text: Volver
    - main [ref=e40]:
      - generic [ref=e42]:
        - generic [ref=e43]:
          - generic [ref=e44]:
            - heading "FC Derby Norte 1 - 0 Atlético Sur CF" [level=3] [ref=e45]
            - generic [ref=e46]: Finalizado
          - generic [ref=e47]:
            - generic [ref=e48]:
              - generic [ref=e49]: 
              - text: 04/06/2026
            - generic [ref=e50]:
              - generic [ref=e51]: 
              - text: 12:49
            - generic [ref=e52]:
              - generic [ref=e53]: 
              - text: Estadio El Pinar
          - button " Detalles" [ref=e55] [cursor=pointer]:
            - generic [ref=e56]: 
            - text: Detalles
        - generic [ref=e57]:
          - generic [ref=e58]:
            - heading "FC Derby Norte 2 - 0 CD Las Torres" [level=3] [ref=e59]
            - generic [ref=e60]: Finalizado
          - generic [ref=e61]:
            - generic [ref=e62]:
              - generic [ref=e63]: 
              - text: 21/05/2026
            - generic [ref=e64]:
              - generic [ref=e65]: 
              - text: 12:49
            - generic [ref=e66]:
              - generic [ref=e67]: 
              - text: Estadio El Pinar
          - button " Detalles" [ref=e69] [cursor=pointer]:
            - generic [ref=e70]: 
            - text: Detalles
        - generic [ref=e71]:
          - generic [ref=e72]:
            - heading "Atlético Sur CF 1 - 0 CD Las Torres" [level=3] [ref=e73]
            - generic [ref=e74]: Finalizado
          - generic [ref=e75]:
            - generic [ref=e76]:
              - generic [ref=e77]: 
              - text: 14/05/2026
            - generic [ref=e78]:
              - generic [ref=e79]: 
              - text: 12:49
            - generic [ref=e80]:
              - generic [ref=e81]: 
              - text: Campo La Ribera
          - button " Detalles" [ref=e83] [cursor=pointer]:
            - generic [ref=e84]: 
            - text: Detalles
        - generic [ref=e85]:
          - generic [ref=e86]:
            - heading "FC Derby Norte 2 - 1 Atlético Sur CF" [level=3] [ref=e87]
            - generic [ref=e88]: Finalizado
          - generic [ref=e89]:
            - generic [ref=e90]:
              - generic [ref=e91]: 
              - text: 07/05/2026
            - generic [ref=e92]:
              - generic [ref=e93]: 
              - text: 12:49
            - generic [ref=e94]:
              - generic [ref=e95]: 
              - text: Estadio El Pinar
          - button " Detalles" [ref=e97] [cursor=pointer]:
            - generic [ref=e98]: 
            - text: Detalles
```