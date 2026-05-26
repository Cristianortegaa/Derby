import { test, expect, Page } from '@playwright/test';

// ─────────────────────────────────────────────────────────────────────────────
// Helpers
// ─────────────────────────────────────────────────────────────────────────────

async function irALogin(page: Page) {
  await page.goto('/login');
  await page.waitForLoadState('networkidle');
}

async function rellenarLogin(page: Page, email: string, password: string) {
  await page.getByLabel(/email/i).fill(email);
  await page.getByLabel(/contraseña/i).fill(password);
}

async function hacerClick(page: Page, texto: string | RegExp) {
  await page.getByRole('button', { name: texto }).click();
}

// ─────────────────────────────────────────────────────────────────────────────
// Tests de la página de login
// ─────────────────────────────────────────────────────────────────────────────

test.describe('Página de Login', () => {

  test.beforeEach(async ({ page }) => {
    await irALogin(page);
  });

  // ── Visualización ──────────────────────────────────────────────────────────

  test('muestra el formulario de login con campos email y contraseña', async ({ page }) => {
    await expect(page.getByLabel(/email/i)).toBeVisible();
    await expect(page.getByLabel(/contraseña/i)).toBeVisible();
    await expect(page.getByRole('button', { name: /iniciar sesión/i })).toBeVisible();
  });

  // ── Validaciones de formulario ─────────────────────────────────────────────

  test('no navega si el formulario está vacío (muestra errores de validación)', async ({ page }) => {
    await hacerClick(page, /iniciar sesión/i);
    // Sigue en la misma página
    await expect(page).toHaveURL('/login');
  });

  test('no envía si el email no tiene formato válido', async ({ page }) => {
    await page.getByLabel(/email/i).fill('noesvalido');
    await page.getByLabel(/contraseña/i).fill('Password1');
    await hacerClick(page, /iniciar sesión/i);
    await expect(page).toHaveURL('/login');
  });

  test('no envía si la contraseña es demasiado corta', async ({ page }) => {
    await page.getByLabel(/email/i).fill('user@derby.com');
    await page.getByLabel(/contraseña/i).fill('abc');
    await hacerClick(page, /iniciar sesión/i);
    await expect(page).toHaveURL('/login');
  });

  // ── Login correcto — Admin ──────────────────────────────────────────────────

  test('login correcto como Admin redirige a /admin', async ({ page }) => {
    await rellenarLogin(page, 'admin@derby.com', 'Password1');
    await hacerClick(page, /iniciar sesión/i);

    // Espera la redirección (se hace con setTimeout(2000) en el código)
    await page.waitForURL('**/admin', { timeout: 10_000 });
    await expect(page).toHaveURL(/\/admin/);
  });

  // ── Login correcto — Árbitro ────────────────────────────────────────────────

  test('login correcto como Árbitro redirige a /arbitro', async ({ page }) => {
    await rellenarLogin(page, 'arbitro@derby.com', 'Password1');
    await hacerClick(page, /iniciar sesión/i);

    await page.waitForURL('**/arbitro', { timeout: 10_000 });
    await expect(page).toHaveURL(/\/arbitro/);
  });

  // ── Login con datos incorrectos ────────────────────────────────────────────

  test('login con credenciales incorrectas muestra mensaje de error', async ({ page }) => {
    await rellenarLogin(page, 'noexiste@derby.com', 'Password1');
    await hacerClick(page, /iniciar sesión/i);

    // El modal de error debe aparecer
    await expect(page.getByText(/credenciales inválidas|error/i)).toBeVisible({ timeout: 8_000 });
    await expect(page).toHaveURL('/login');
  });

  // ── Protección de rutas ────────────────────────────────────────────────────

  test('acceder a /admin sin sesión redirige a /login', async ({ page }) => {
    await page.goto('/admin');
    await page.waitForURL('**/login', { timeout: 10_000 });
    await expect(page).toHaveURL(/\/login/);
  });

  test('acceder a /arbitro sin sesión redirige a /login', async ({ page }) => {
    await page.goto('/arbitro');
    await page.waitForURL('**/login', { timeout: 10_000 });
    await expect(page).toHaveURL(/\/login/);
  });

  // ── Toggle registro ────────────────────────────────────────────────────────

  test('botón de registro muestra el formulario de registro', async ({ page }) => {
    const botonRegistro = page.getByRole('button', { name: /regíst|crear cuenta/i });
    if (await botonRegistro.isVisible()) {
      await botonRegistro.click();
      await expect(page.getByRole('button', { name: /registrarse/i })).toBeVisible();
    }
  });
});
