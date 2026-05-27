import { test, expect, Page } from '@playwright/test';

// ─────────────────────────────────────────────────────────────────────────────
// Helper: login como árbitro
// ─────────────────────────────────────────────────────────────────────────────

async function loginComoArbitro(page: Page) {
  await page.goto('/login');
  await page.waitForLoadState('domcontentloaded');
  await page.locator('input[type="email"]').fill('arbitro1@derby.com');
  await page.locator('input[type="password"]').fill('Arbitro@123');
  const btn = page.locator('button[type="submit"]');
  await expect(btn).not.toBeDisabled({ timeout: 5_000 });
  await btn.click();
  await page.waitForURL('**/arbitro', { timeout: 15_000 });
}

// ─────────────────────────────────────────────────────────────────────────────
// Tests del panel del árbitro
// ─────────────────────────────────────────────────────────────────────────────

test.describe('Panel del Árbitro', () => {

  test.beforeEach(async ({ page }) => {
    await loginComoArbitro(page);
  });

  // 1. El dashboard se muestra tras el login
  test('el dashboard del árbitro se muestra tras el login', async ({ page }) => {
    await expect(page).toHaveURL(/\/arbitro/);
    await expect(page.locator('body')).not.toBeEmpty();
  });

  // 2. Mis Partidos muestra los partidos asignados
  test('Mis Partidos muestra los partidos asignados', async ({ page }) => {
    await page.goto('/arbitro/mis-partidos');
    await page.waitForLoadState('domcontentloaded');
    await expect(page.getByRole('heading', { name: /mis partidos/i })).toBeVisible({ timeout: 5_000 });
    await expect(page.locator('a[href*="/arbitro/acta"]').first()).toBeVisible({ timeout: 8_000 });
  });

  // 3. Añade un evento al acta y la cierra
  test('puede añadir un evento y cerrar el acta', async ({ page }) => {
    // Entra al acta del primer partido pendiente
    await page.goto('/arbitro/mis-partidos');
    await page.waitForLoadState('domcontentloaded');
    await page.locator('a[href*="/arbitro/acta"]').first().click();
    await page.waitForURL(/\/arbitro\/acta\/\d+/, { timeout: 8_000 });
    await expect(page.getByRole('heading', { name: /acta del partido/i })).toBeVisible({ timeout: 5_000 });

    // Abre el dropdown y selecciona el primer jugador
    await page.locator('button:has-text("Selecciona jugador")').click();
    await page.locator('.absolute.z-30 div[class*="cursor-pointer"]').first().click();

    // Rellena el minuto y añade el evento
    await page.locator('input[type="number"]').fill('10');
    await page.getByRole('button', { name: /añadir evento/i }).click();

    // Verifica que el evento aparece en la lista
    await expect(page.locator('text=min. 10').first()).toBeVisible({ timeout: 8_000 });

    // Cierra el acta → se abre modal de confirmación
    await page.getByRole('button', { name: /cerrar acta/i }).click();
    await expect(page.getByText('¿Seguro que quieres cerrar el acta?')).toBeVisible({ timeout: 5_000 });

    // Confirma en el modal → redirige a historial
    await page.locator('a:has-text("Cerrar Acta")').click();
    await page.waitForURL('**/arbitro/historial', { timeout: 10_000 });
    await expect(page).toHaveURL(/\/arbitro\/historial/);
  });

  // 4. En historial abre detalles de un partido y cierra el modal
  // Este test usa el partido que cerró el test 3
  test('en el historial puede abrir y cerrar el modal de detalles', async ({ page }) => {
    await page.goto('/arbitro/historial');
    await page.waitForLoadState('networkidle');

    // Hace click en el botón Detalles del primer partido
    await page.locator('button').filter({ hasText: 'Detalles' }).first().click();

    // Verifica que el modal se abre
    await expect(page.getByText('Detalles del partido')).toBeVisible({ timeout: 5_000 });

    // Cierra el modal
    await page.locator('button').filter({ hasText: 'Cerrar' }).click();

    // Verifica que el modal desaparece
    await expect(page.getByText('Detalles del partido')).not.toBeVisible({ timeout: 5_000 });
  });

});
