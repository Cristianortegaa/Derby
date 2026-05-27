import { test, expect, Page } from '@playwright/test';

// ─────────────────────────────────────────────────────────────────────────────
// Helper: login como admin
// ─────────────────────────────────────────────────────────────────────────────

async function loginComoAdmin(page: Page) {
  await page.goto('/login');
  await page.waitForLoadState('domcontentloaded');
  await page.locator('input[type="email"]').fill('admin@derby.com');
  await page.locator('input[type="password"]').fill('Admin@123');
  const btn = page.locator('button[type="submit"]');
  await expect(btn).not.toBeDisabled({ timeout: 5_000 });
  await btn.click();
  await page.waitForURL('**/admin', { timeout: 15_000 });
}

// ─────────────────────────────────────────────────────────────────────────────
// Tests del panel de administración
// ─────────────────────────────────────────────────────────────────────────────

test.describe('Panel de Administración', () => {

  test.beforeEach(async ({ page }) => {
    await loginComoAdmin(page);
  });

  // 1. Crea una competición y verifica que aparece en la tabla
  test('puede crear una nueva competición', async ({ page }) => {
    await page.goto('/admin/competiciones');
    await page.waitForLoadState('domcontentloaded');

    // Abre el formulario
    await page.getByRole('button', { name: /nueva/i }).click();
    await expect(page.getByText('Nueva competición')).toBeVisible({ timeout: 5_000 });

    // Rellena los campos obligatorios
    await page.getByPlaceholder(/Copa RFEF/i).fill('Competicion Test E2E');
    await page.getByPlaceholder(/2025-2026/i).fill('2025-2026');

    // Guarda
    await page.locator('button.btn-primary').filter({ hasText: 'Crear' }).click();

    // Verifica que aparece en la tabla
    await expect(page.getByText('Competicion Test E2E')).toBeVisible({ timeout: 8_000 });
  });

  // 2. Edita la competición creada y verifica el nombre actualizado
  test('puede editar una competición existente', async ({ page }) => {
    await page.goto('/admin/competiciones');
    await page.waitForLoadState('networkidle');

    // Encuentra la fila de la competición y hace click en editar
    const fila = page.locator('tr').filter({ hasText: 'Competicion Test E2E' });
    await fila.locator('button[title="Editar"]').click();
    await expect(page.getByText('Editar competición')).toBeVisible({ timeout: 5_000 });

    // Cambia el nombre
    const campoNombre = page.getByPlaceholder(/Copa RFEF/i);
    await campoNombre.clear();
    await campoNombre.fill('Competicion E2E Editada');

    // Guarda
    await page.getByRole('button', { name: /actualizar/i }).click();

    // Verifica el nombre actualizado en la tabla
    await expect(page.getByText('Competicion E2E Editada')).toBeVisible({ timeout: 8_000 });
  });

  // 3. Filtra usuarios por rol y busca por email en el mismo test
  test('puede filtrar usuarios por rol y buscar por email', async ({ page }) => {
    await page.goto('/admin/usuarios');
    await page.waitForLoadState('networkidle');

    // Filtra por Árbitro y verifica que aparece arbitro1
    await page.locator('.filter-arbitro').click();
    await expect(page.getByText('arbitro1@derby.com')).toBeVisible({ timeout: 5_000 });

    // Limpia el filtro y busca admin por email
    await page.locator('.filter-todos').click();
    await page.getByPlaceholder('Buscar por email...').fill('admin@derby.com');
    await expect(page.getByText('admin@derby.com')).toBeVisible({ timeout: 5_000 });
  });

  // 4. Edita el primer equipo de la lista cambiando la sede
  test('puede editar un equipo existente', async ({ page }) => {
    await page.goto('/admin/equipos');
    await page.waitForLoadState('networkidle');

    // Hace click en editar del primer equipo
    await page.locator('button[title="Editar"]').first().click();
    await expect(page.getByText('Editar Equipo')).toBeVisible({ timeout: 5_000 });

    // Cambia la sede
    const campoSede = page.getByPlaceholder('Sede');
    await campoSede.clear();
    await campoSede.fill('Sede E2E Test');

    // Guarda
    await page.getByRole('button', { name: /actualizar/i }).click();

    // Verifica que el formulario se cierra (indica éxito)
    await expect(page.getByText('Editar Equipo')).not.toBeVisible({ timeout: 5_000 });
  });

});
