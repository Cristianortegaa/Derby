import { test, expect, Page } from '@playwright/test';

// ─────────────────────────────────────────────────────────────────────────────
// Helpers
// ─────────────────────────────────────────────────────────────────────────────

async function irALogin(page: Page) {
  await page.goto('/login');
  await page.waitForLoadState('domcontentloaded');
}

async function hacerLogin(page: Page, email: string, password: string) {
  await page.locator('input[type="email"]').fill(email);
  await page.locator('input[type="password"]').fill(password);
  const btn = page.locator('button[type="submit"]');
  await expect(btn).not.toBeDisabled({ timeout: 5_000 });
  await btn.click();
}

// ─────────────────────────────────────────────────────────────────────────────
// Tests de Login
// ─────────────────────────────────────────────────────────────────────────────

test.describe('Login', () => {

  test.beforeEach(async ({ page }) => {
    await irALogin(page);
  });

  // 1. El formulario se muestra correctamente
  test('muestra el formulario con email, contraseña y botón', async ({ page }) => {
    await expect(page.locator('input[type="email"]')).toBeVisible();
    await expect(page.locator('input[type="password"]')).toBeVisible();
    await expect(page.locator('button[type="submit"]')).toBeVisible();
  });

  // 2. Login correcto redirige al dashboard
  test('login correcto como Admin redirige a /admin', async ({ page }) => {
    await hacerLogin(page, 'admin@derby.com', 'Admin@123');
    await page.waitForURL('**/admin', { timeout: 15_000 });
    await expect(page).toHaveURL(/\/admin/);
  });

  // 3. Credenciales incorrectas muestran error
  test('credenciales incorrectas muestran mensaje de error', async ({ page }) => {
    await hacerLogin(page, 'noexiste@derby.com', 'Admin@123');
    await expect(page.locator('.fixed.inset-0')).toBeVisible({ timeout: 8_000 });
    await expect(page).toHaveURL('/login');
  });

  // 4. Ruta protegida sin sesión redirige a /login
  test('acceder a /admin sin sesión redirige a /login', async ({ page }) => {
    await page.goto('/admin');
    await page.waitForURL('**/login', { timeout: 10_000 });
    await expect(page).toHaveURL(/\/login/);
  });

});
