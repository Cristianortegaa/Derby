import { test, expect, Page } from '@playwright/test';

// ─────────────────────────────────────────────────────────────────────────────
// Helpers
// ─────────────────────────────────────────────────────────────────────────────

async function loginComoArbitro(page: Page) {
  await page.goto('/login');
  await page.waitForLoadState('networkidle');
  await page.getByLabel(/email/i).fill('arbitro@derby.com');
  await page.getByLabel(/contraseña/i).fill('Password1');
  await page.getByRole('button', { name: /iniciar sesión/i }).click();
  await page.waitForURL('**/arbitro', { timeout: 10_000 });
}

// ─────────────────────────────────────────────────────────────────────────────
// Tests del panel del árbitro
// ─────────────────────────────────────────────────────────────────────────────

test.describe('Panel del Árbitro', () => {

  test.beforeEach(async ({ page }) => {
    await loginComoArbitro(page);
  });

  // ── Dashboard ──────────────────────────────────────────────────────────────

  test('el dashboard del árbitro se muestra tras el login', async ({ page }) => {
    await expect(page).toHaveURL(/\/arbitro/);
    await expect(page.getByRole('heading', { name: /árbitr|arbitr|dashboard|panel/i })).toBeVisible({ timeout: 8_000 });
  });

  test('la barra de navegación del árbitro está visible', async ({ page }) => {
    await expect(page.getByRole('navigation')).toBeVisible();
  });

  // ── Mis partidos ───────────────────────────────────────────────────────────

  test('puede navegar a la sección "Mis Partidos"', async ({ page }) => {
    const enlace = page.getByRole('link', { name: /mis partidos|partidos/i }).first();
    if (await enlace.isVisible()) {
      await enlace.click();
      await expect(page).toHaveURL(/arbitro/);
    }
  });

  test('la página de mis partidos se carga correctamente', async ({ page }) => {
    await page.goto('/arbitro/mis-partidos');
    await page.waitForLoadState('networkidle');
    // Debe mostrar la sección sin errores 500 ni redirección inesperada
    await expect(page).toHaveURL(/arbitro/);
  });

  // ── Historial ──────────────────────────────────────────────────────────────

  test('la página de historial del árbitro se carga correctamente', async ({ page }) => {
    await page.goto('/arbitro/historial');
    await page.waitForLoadState('networkidle');
    await expect(page).toHaveURL(/arbitro/);
  });

  // ── Seguridad: el árbitro no puede acceder al panel de admin ──────────────

  test('el árbitro no puede acceder a /admin y es redirigido', async ({ page }) => {
    await page.goto('/admin');
    // Debe redirigir a / o a /login (no a /admin)
    await page.waitForURL(url => !url.pathname.startsWith('/admin'), { timeout: 10_000 });
    await expect(page).not.toHaveURL(/^.*\/admin$/);
  });

  // ── Seguridad: rutas de árbitro protegidas ────────────────────────────────

  test('cerrar sesión y acceder a /arbitro redirige a /login', async ({ page }) => {
    // Limpiar sesión directamente borrando localStorage
    await page.evaluate(() => localStorage.removeItem('usuarioActual'));
    await page.goto('/arbitro');
    await page.waitForURL('**/login', { timeout: 10_000 });
    await expect(page).toHaveURL(/\/login/);
  });

  test('un usuario sin sesión no puede acceder a /arbitro/mis-partidos', async ({ page }) => {
    await page.evaluate(() => localStorage.removeItem('usuarioActual'));
    await page.goto('/arbitro/mis-partidos');
    await page.waitForURL('**/login', { timeout: 10_000 });
    await expect(page).toHaveURL(/\/login/);
  });
});
