import { test, expect, Page } from '@playwright/test';

// ─────────────────────────────────────────────────────────────────────────────
// Helpers
// ─────────────────────────────────────────────────────────────────────────────

async function loginComoAdmin(page: Page) {
  await page.goto('/login');
  await page.waitForLoadState('networkidle');
  await page.getByLabel(/email/i).fill('admin@derby.com');
  await page.getByLabel(/contraseña/i).fill('Password1');
  await page.getByRole('button', { name: /iniciar sesión/i }).click();
  await page.waitForURL('**/admin', { timeout: 10_000 });
}

// ─────────────────────────────────────────────────────────────────────────────
// Tests del panel de administración
// ─────────────────────────────────────────────────────────────────────────────

test.describe('Panel de Administración', () => {

  test.beforeEach(async ({ page }) => {
    await loginComoAdmin(page);
  });

  // ── Dashboard ──────────────────────────────────────────────────────────────

  test('el dashboard de admin se muestra tras el login', async ({ page }) => {
    await expect(page).toHaveURL(/\/admin/);
    await expect(page.getByRole('heading', { name: /dashboard|panel|administr/i })).toBeVisible({ timeout: 8_000 });
  });

  test('la barra de navegación del admin está visible', async ({ page }) => {
    await expect(page.getByRole('navigation')).toBeVisible();
  });

  // ── Competiciones ──────────────────────────────────────────────────────────

  test('puede navegar a la sección de competiciones', async ({ page }) => {
    await page.getByRole('link', { name: /competicion/i }).first().click();
    await page.waitForURL('**/competiciones', { timeout: 8_000 });
    await expect(page).toHaveURL(/\/competiciones/);
  });

  test('la página de competiciones muestra la tabla o lista de competiciones', async ({ page }) => {
    await page.goto('/admin/competiciones');
    await page.waitForLoadState('networkidle');
    await expect(page.getByRole('heading', { name: /competicion/i })).toBeVisible({ timeout: 8_000 });
  });

  test('el botón "Nueva Competición" abre el formulario', async ({ page }) => {
    await page.goto('/admin/competiciones');
    await page.waitForLoadState('networkidle');
    const boton = page.getByRole('button', { name: /nueva|crear|añadir/i }).first();
    await boton.click();
    await expect(page.getByRole('textbox', { name: /nombre/i }).first()).toBeVisible({ timeout: 5_000 });
  });

  // ── Ligas ──────────────────────────────────────────────────────────────────

  test('la página de ligas se carga correctamente', async ({ page }) => {
    await page.goto('/admin/ligas');
    await page.waitForLoadState('networkidle');
    await expect(page.getByRole('heading', { name: /liga/i })).toBeVisible({ timeout: 8_000 });
  });

  test('el botón "Nueva Liga" abre el formulario', async ({ page }) => {
    await page.goto('/admin/ligas');
    await page.waitForLoadState('networkidle');
    const boton = page.getByRole('button', { name: /nueva|crear|añadir/i }).first();
    await boton.click();
    await expect(page.getByRole('textbox', { name: /nombre/i }).first()).toBeVisible({ timeout: 5_000 });
  });

  // ── Equipos ────────────────────────────────────────────────────────────────

  test('la página de equipos se carga correctamente', async ({ page }) => {
    await page.goto('/admin/equipos');
    await page.waitForLoadState('networkidle');
    await expect(page.getByRole('heading', { name: /equipo/i })).toBeVisible({ timeout: 8_000 });
  });

  // ── Partidos ───────────────────────────────────────────────────────────────

  test('la página de partidos se carga correctamente', async ({ page }) => {
    await page.goto('/admin/partidos');
    await page.waitForLoadState('networkidle');
    await expect(page.getByRole('heading', { name: /partido/i })).toBeVisible({ timeout: 8_000 });
  });

  // ── Usuarios ───────────────────────────────────────────────────────────────

  test('la página de usuarios se carga correctamente', async ({ page }) => {
    await page.goto('/admin/usuarios');
    await page.waitForLoadState('networkidle');
    await expect(page.getByRole('heading', { name: /usuario/i })).toBeVisible({ timeout: 8_000 });
  });

  test('la lista de usuarios muestra al menos el propio admin', async ({ page }) => {
    await page.goto('/admin/usuarios');
    await page.waitForLoadState('networkidle');
    await expect(page.getByText('admin@derby.com')).toBeVisible({ timeout: 8_000 });
  });

  // ── Árbitros ───────────────────────────────────────────────────────────────

  test('la página de árbitros se carga correctamente', async ({ page }) => {
    await page.goto('/admin/arbitros');
    await page.waitForLoadState('networkidle');
    await expect(page.getByRole('heading', { name: /árbitr|arbitr/i })).toBeVisible({ timeout: 8_000 });
  });

  // ── Seguridad: rutas de admin protegidas ───────────────────────────────────

  test('cerrar sesión y acceder a /admin redirige a /login', async ({ page }) => {
    // Limpiar sesión directamente borrando localStorage
    await page.evaluate(() => localStorage.removeItem('usuarioActual'));
    await page.goto('/admin');
    await page.waitForURL('**/login', { timeout: 10_000 });
    await expect(page).toHaveURL(/\/login/);
  });
});
