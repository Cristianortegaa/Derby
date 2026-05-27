# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: admin.spec.ts >> Panel de Administración >> puede editar una competición existente
- Location: tests\admin.spec.ts:49:7

# Error details

```
Error: expect(locator).toBeVisible() failed

Locator: getByText('Competicion E2E Editada')
Expected: visible
Error: strict mode violation: getByText('Competicion E2E Editada') resolved to 2 elements:
    1) <td _ngcontent-ng-c3133320202="" class="td-cell font-semibold text-white">Competicion E2E Editada</td> aka getByRole('cell', { name: 'Competicion E2E Editada' }).first()
    2) <td _ngcontent-ng-c3133320202="" class="td-cell font-semibold text-white">Competicion E2E Editada</td> aka getByRole('cell', { name: 'Competicion E2E Editada' }).nth(1)

Call log:
  - Expect "toBeVisible" with timeout 8000ms
  - waiting for getByText('Competicion E2E Editada')

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
        - button " admin@derby.com " [ref=e26] [cursor=pointer]:
          - generic [ref=e27]:
            - generic [ref=e28]: 
            - generic [ref=e29]: admin@derby.com
          - generic [ref=e30]: 
    - generic [ref=e32]:
      - generic [ref=e33]:
        - generic [ref=e34]:
          - heading "Gestionar Competiciones" [level=1] [ref=e35]
          - paragraph [ref=e36]: Crea, edita y gestiona competiciones y temporadas
        - link " Volver" [ref=e39] [cursor=pointer]:
          - /url: /admin
          - generic [ref=e40]: 
          - text: Volver
      - generic [ref=e41]:
        - generic [ref=e42]:
          - generic [ref=e43]:
            - generic [ref=e44]: 
            - text: Competiciones
          - generic [ref=e45]: "6"
          - generic [ref=e46]: totales en el sistema
        - generic [ref=e47]:
          - generic [ref=e48]:
            - generic [ref=e49]: 
            - text: Activas
          - generic [ref=e50]: "6"
          - generic [ref=e51]: en curso ahora mismo
        - generic [ref=e52]:
          - generic [ref=e53]:
            - generic [ref=e54]: 
            - text: Inactivas
          - generic [ref=e55]: "0"
          - generic [ref=e56]: pausadas o finalizadas
      - generic [ref=e57]:
        - generic [ref=e58]:
          - heading " Competiciones (6)" [level=2] [ref=e59]:
            - generic [ref=e60]: 
            - text: Competiciones (6)
          - generic [ref=e61]:
            - generic [ref=e62]:
              - generic [ref=e63]: 
              - textbox "Buscar por nombre..." [ref=e64]
            - combobox [ref=e65] [cursor=pointer]:
              - option "Todos los estados" [selected]
              - option "Activo"
              - option "Inactivo"
              - option "Pausado"
              - option "Finalizado"
            - button "+ Nueva" [ref=e66] [cursor=pointer]:
              - generic [ref=e67]: +
              - text: Nueva
        - table [ref=e69]:
          - rowgroup [ref=e70]:
            - row "Nombre Temporada Tipo Tipo Juego Estado Acciones" [ref=e71]:
              - columnheader "Nombre" [ref=e72]
              - columnheader "Temporada" [ref=e73]
              - columnheader "Tipo" [ref=e74]
              - columnheader "Tipo Juego" [ref=e75]
              - columnheader "Estado" [ref=e76]
              - columnheader "Acciones" [ref=e77]
          - rowgroup [ref=e78]:
            - row "Liga Derby 2025-2026 Liga futbol11 Activo   " [ref=e79]:
              - cell "Liga Derby" [ref=e80]
              - cell "2025-2026" [ref=e81]
              - cell "Liga" [ref=e82]:
                - generic [ref=e83]: Liga
              - cell "futbol11" [ref=e84]:
                - generic [ref=e85]: futbol11
              - cell "Activo" [ref=e86]:
                - generic [ref=e87]: Activo
              - cell "  " [ref=e89]:
                - generic [ref=e90]:
                  - link "" [ref=e91] [cursor=pointer]:
                    - /url: /admin/competiciones/1
                    - generic [ref=e92]: 
                  - button "" [ref=e93] [cursor=pointer]:
                    - generic [ref=e94]: 
                  - button "" [ref=e95] [cursor=pointer]:
                    - generic [ref=e96]: 
            - row "Copa Derby 2025-2026 Liga futbol11 Activo   " [ref=e97]:
              - cell "Copa Derby" [ref=e98]
              - cell "2025-2026" [ref=e99]
              - cell "Liga" [ref=e100]:
                - generic [ref=e101]: Liga
              - cell "futbol11" [ref=e102]:
                - generic [ref=e103]: futbol11
              - cell "Activo" [ref=e104]:
                - generic [ref=e105]: Activo
              - cell "  " [ref=e107]:
                - generic [ref=e108]:
                  - link "" [ref=e109] [cursor=pointer]:
                    - /url: /admin/competiciones/2
                    - generic [ref=e110]: 
                  - button "" [ref=e111] [cursor=pointer]:
                    - generic [ref=e112]: 
                  - button "" [ref=e113] [cursor=pointer]:
                    - generic [ref=e114]: 
            - row "Torneo Verano 2025-2026 Liga futbol7 Activo   " [ref=e115]:
              - cell "Torneo Verano" [ref=e116]
              - cell "2025-2026" [ref=e117]
              - cell "Liga" [ref=e118]:
                - generic [ref=e119]: Liga
              - cell "futbol7" [ref=e120]:
                - generic [ref=e121]: futbol7
              - cell "Activo" [ref=e122]:
                - generic [ref=e123]: Activo
              - cell "  " [ref=e125]:
                - generic [ref=e126]:
                  - link "" [ref=e127] [cursor=pointer]:
                    - /url: /admin/competiciones/3
                    - generic [ref=e128]: 
                  - button "" [ref=e129] [cursor=pointer]:
                    - generic [ref=e130]: 
                  - button "" [ref=e131] [cursor=pointer]:
                    - generic [ref=e132]: 
            - row "Competicion E2E Editada 2025-2026 Liga Futbol-11 Activo   " [ref=e133]:
              - cell "Competicion E2E Editada" [ref=e134]
              - cell "2025-2026" [ref=e135]
              - cell "Liga" [ref=e136]:
                - generic [ref=e137]: Liga
              - cell "Futbol-11" [ref=e138]:
                - generic [ref=e139]: Futbol-11
              - cell "Activo" [ref=e140]:
                - generic [ref=e141]: Activo
              - cell "  " [ref=e143]:
                - generic [ref=e144]:
                  - link "" [ref=e145] [cursor=pointer]:
                    - /url: /admin/competiciones/4
                    - generic [ref=e146]: 
                  - button "" [ref=e147] [cursor=pointer]:
                    - generic [ref=e148]: 
                  - button "" [ref=e149] [cursor=pointer]:
                    - generic [ref=e150]: 
            - row "Competicion E2E Editada 2025-2026 Liga Futbol-11 Activo   " [ref=e151]:
              - cell "Competicion E2E Editada" [ref=e152]
              - cell "2025-2026" [ref=e153]
              - cell "Liga" [ref=e154]:
                - generic [ref=e155]: Liga
              - cell "Futbol-11" [ref=e156]:
                - generic [ref=e157]: Futbol-11
              - cell "Activo" [ref=e158]:
                - generic [ref=e159]: Activo
              - cell "  " [ref=e161]:
                - generic [ref=e162]:
                  - link "" [ref=e163] [cursor=pointer]:
                    - /url: /admin/competiciones/5
                    - generic [ref=e164]: 
                  - button "" [ref=e165] [cursor=pointer]:
                    - generic [ref=e166]: 
                  - button "" [ref=e167] [cursor=pointer]:
                    - generic [ref=e168]: 
            - row "Competicion E2E Editada 2025-2026 Liga Futbol-11 Activo   " [ref=e169]:
              - cell "Competicion E2E Editada" [ref=e170]
              - cell "2025-2026" [ref=e171]
              - cell "Liga" [ref=e172]:
                - generic [ref=e173]: Liga
              - cell "Futbol-11" [ref=e174]:
                - generic [ref=e175]: Futbol-11
              - cell "Activo" [ref=e176]:
                - generic [ref=e177]: Activo
              - cell "  " [ref=e179]:
                - generic [ref=e180]:
                  - link "" [ref=e181] [cursor=pointer]:
                    - /url: /admin/competiciones/6
                    - generic [ref=e182]: 
                  - button "" [ref=e183] [cursor=pointer]:
                    - generic [ref=e184]: 
                  - button "" [ref=e185] [cursor=pointer]:
                    - generic [ref=e186]: 
    - generic [ref=e187]:
      - generic [ref=e188]: 
      - text: Competición actualizada correctamente
```

# Test source

```ts
  1   | import { test, expect, Page } from '@playwright/test';
  2   | 
  3   | // ─────────────────────────────────────────────────────────────────────────────
  4   | // Helper: login como admin
  5   | // ─────────────────────────────────────────────────────────────────────────────
  6   | 
  7   | async function loginComoAdmin(page: Page) {
  8   |   await page.goto('/login');
  9   |   await page.waitForLoadState('domcontentloaded');
  10  |   await page.locator('input[type="email"]').fill('admin@derby.com');
  11  |   await page.locator('input[type="password"]').fill('Admin@123');
  12  |   const btn = page.locator('button[type="submit"]');
  13  |   await expect(btn).not.toBeDisabled({ timeout: 5_000 });
  14  |   await btn.click();
  15  |   await page.waitForURL('**/admin', { timeout: 15_000 });
  16  | }
  17  | 
  18  | // ─────────────────────────────────────────────────────────────────────────────
  19  | // Tests del panel de administración
  20  | // ─────────────────────────────────────────────────────────────────────────────
  21  | 
  22  | test.describe('Panel de Administración', () => {
  23  | 
  24  |   test.beforeEach(async ({ page }) => {
  25  |     await loginComoAdmin(page);
  26  |   });
  27  | 
  28  |   // 1. Crea una competición y verifica que aparece en la tabla
  29  |   test('puede crear una nueva competición', async ({ page }) => {
  30  |     await page.goto('/admin/competiciones');
  31  |     await page.waitForLoadState('domcontentloaded');
  32  | 
  33  |     // Abre el formulario
  34  |     await page.getByRole('button', { name: /nueva/i }).click();
  35  |     await expect(page.getByText('Nueva competición')).toBeVisible({ timeout: 5_000 });
  36  | 
  37  |     // Rellena los campos obligatorios
  38  |     await page.getByPlaceholder(/Copa RFEF/i).fill('Competicion Test E2E');
  39  |     await page.getByPlaceholder(/2025-2026/i).fill('2025-2026');
  40  | 
  41  |     // Guarda
  42  |     await page.locator('button.btn-primary').filter({ hasText: 'Crear' }).click();
  43  | 
  44  |     // Verifica que aparece en la tabla
  45  |     await expect(page.getByText('Competicion Test E2E')).toBeVisible({ timeout: 8_000 });
  46  |   });
  47  | 
  48  |   // 2. Edita la competición creada y verifica el nombre actualizado
  49  |   test('puede editar una competición existente', async ({ page }) => {
  50  |     await page.goto('/admin/competiciones');
  51  |     await page.waitForLoadState('networkidle');
  52  | 
  53  |     // Encuentra la fila de la competición y hace click en editar
  54  |     const fila = page.locator('tr').filter({ hasText: 'Competicion Test E2E' });
  55  |     await fila.locator('button[title="Editar"]').click();
  56  |     await expect(page.getByText('Editar competición')).toBeVisible({ timeout: 5_000 });
  57  | 
  58  |     // Cambia el nombre
  59  |     const campoNombre = page.getByPlaceholder(/Copa RFEF/i);
  60  |     await campoNombre.clear();
  61  |     await campoNombre.fill('Competicion E2E Editada');
  62  | 
  63  |     // Guarda
  64  |     await page.getByRole('button', { name: /actualizar/i }).click();
  65  | 
  66  |     // Verifica el nombre actualizado en la tabla
> 67  |     await expect(page.getByText('Competicion E2E Editada')).toBeVisible({ timeout: 8_000 });
      |                                                             ^ Error: expect(locator).toBeVisible() failed
  68  |   });
  69  | 
  70  |   // 3. Filtra usuarios por rol y busca por email en el mismo test
  71  |   test('puede filtrar usuarios por rol y buscar por email', async ({ page }) => {
  72  |     await page.goto('/admin/usuarios');
  73  |     await page.waitForLoadState('networkidle');
  74  | 
  75  |     // Filtra por Árbitro y verifica que aparece arbitro1
  76  |     await page.locator('.filter-arbitro').click();
  77  |     await expect(page.getByText('arbitro1@derby.com')).toBeVisible({ timeout: 5_000 });
  78  | 
  79  |     // Limpia el filtro y busca admin por email
  80  |     await page.locator('.filter-todos').click();
  81  |     await page.getByPlaceholder('Buscar por email...').fill('admin@derby.com');
  82  |     await expect(page.getByText('admin@derby.com')).toBeVisible({ timeout: 5_000 });
  83  |   });
  84  | 
  85  |   // 4. Edita el primer equipo de la lista cambiando la sede
  86  |   test('puede editar un equipo existente', async ({ page }) => {
  87  |     await page.goto('/admin/equipos');
  88  |     await page.waitForLoadState('networkidle');
  89  | 
  90  |     // Hace click en editar del primer equipo
  91  |     await page.locator('button[title="Editar"]').first().click();
  92  |     await expect(page.getByText('Editar Equipo')).toBeVisible({ timeout: 5_000 });
  93  | 
  94  |     // Cambia la sede
  95  |     const campoSede = page.getByPlaceholder('Sede');
  96  |     await campoSede.clear();
  97  |     await campoSede.fill('Sede E2E Test');
  98  | 
  99  |     // Guarda
  100 |     await page.getByRole('button', { name: /actualizar/i }).click();
  101 | 
  102 |     // Verifica que el formulario se cierra (indica éxito)
  103 |     await expect(page.getByText('Editar Equipo')).not.toBeVisible({ timeout: 5_000 });
  104 |   });
  105 | 
  106 | });
  107 | 
```