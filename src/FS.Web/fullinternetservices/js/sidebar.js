/**
 * sidebar.js — Shared sidebar renderer for all dashboard pages
 * Call renderSidebar(activeLink) after DOM loaded
 */

function renderSidebar(activeLink, role) {
  const user = Auth.currentUser();
  if (!user) return;

  const clienteLinks = `
    <div class="sidebar__section-label">Mi Cuenta</div>
    <a href="dashboard-cliente.html" class="sidebar__link ${activeLink==='home'?'active':''}" data-link="home">
      <span class="icon">🏠</span> Inicio
    </a>
    <a href="mis-contratos.html" class="sidebar__link ${activeLink==='contratos'?'active':''}" data-link="contratos">
      <span class="icon">📋</span> Mis Contratos
    </a>
    <a href="mis-pagos.html" class="sidebar__link ${activeLink==='pagos'?'active':''}" data-link="pagos">
      <span class="icon">💳</span> Mis Pagos
    </a>
    <a href="mis-reclamos.html" class="sidebar__link ${activeLink==='reclamos'?'active':''}" data-link="reclamos">
      <span class="icon">🎧</span> Mis Reclamos
    </a>
    <div class="sidebar__section-label">Planes</div>
    <a href="planes.html" class="sidebar__link ${activeLink==='planes'?'active':''}" data-link="planes">
      <span class="icon">📡</span> Ver Planes
    </a>
  `;

  const adminLinks = `
    <div class="sidebar__section-label">Panel Admin</div>
    <a href="dashboard-admin.html" class="sidebar__link ${activeLink==='home'?'active':''}" data-link="home">
      <span class="icon">📊</span> Dashboard
    </a>
    <a href="admin-clientes.html" class="sidebar__link ${activeLink==='clientes'?'active':''}" data-link="clientes">
      <span class="icon">👥</span> Clientes
    </a>
    <a href="admin-contratos.html" class="sidebar__link ${activeLink==='contratos'?'active':''}" data-link="contratos">
      <span class="icon">📋</span> Contratos
    </a>
    <a href="admin-pagos.html" class="sidebar__link ${activeLink==='pagos'?'active':''}" data-link="pagos">
      <span class="icon">💳</span> Pagos
    </a>
    <a href="admin-reclamos.html" class="sidebar__link ${activeLink==='reclamos'?'active':''}" data-link="reclamos">
      <span class="icon">🎧</span> Reclamos
    </a>
    <div class="sidebar__section-label">Reportes</div>
    <a href="reportes.html" class="sidebar__link ${activeLink==='reportes'?'active':''}" data-link="reportes">
      <span class="icon">📈</span> Reportes
    </a>
    <a href="bitacora.html" class="sidebar__link ${activeLink==='bitacora'?'active':''}" data-link="bitacora">
      <span class="icon">📒</span> Bitácora
    </a>
    <div class="sidebar__section-label">Catálogo</div>
    <a href="planes.html" class="sidebar__link ${activeLink==='planes'?'active':''}" data-link="planes">
      <span class="icon">📡</span> Planes
    </a>
  `;

  const tecnicoLinks = `
    <div class="sidebar__section-label">Técnico</div>
    <a href="dashboard-tecnico.html" class="sidebar__link ${activeLink==='home'?'active':''}" data-link="home">
      <span class="icon">🔧</span> Mis Reclamos
    </a>
    <a href="planes.html" class="sidebar__link ${activeLink==='planes'?'active':''}" data-link="planes">
      <span class="icon">📡</span> Planes
    </a>
  `;

  const linksByRole = {
    'Cliente':       clienteLinks,
    'Administrador': adminLinks,
    'Cajero':        adminLinks,
    'Tecnico':       tecnicoLinks,
  };

  const nav = linksByRole[user.rol] || clienteLinks;
  const initials = user.nombre.split(' ').map(w=>w[0]).join('').slice(0,2).toUpperCase();

  const sidebar = document.getElementById('sidebar');
  sidebar.innerHTML = `
    <div class="sidebar__logo">
      <div class="sidebar__logo-icon">FI</div>
      <div>
        <div class="sidebar__logo-text">Full Internet</div>
        <div class="sidebar__logo-sub">Services</div>
      </div>
    </div>
    <nav class="sidebar__nav">${nav}</nav>
    <div class="sidebar__user">
      <div class="sidebar__avatar">${initials}</div>
      <div>
        <div class="sidebar__user-name">${user.nombre.split(' ').slice(0,2).join(' ')}</div>
        <div class="sidebar__user-role">${user.rol}</div>
      </div>
      <button class="sidebar__logout" title="Cerrar sesión" onclick="Auth.logout()">⏻</button>
    </div>
  `;

  // Mobile hamburger
  const hamburger = document.getElementById('hamburger');
  if (hamburger) {
    hamburger.onclick = () => sidebar.classList.toggle('open');
    document.addEventListener('click', e => {
      if (!sidebar.contains(e.target) && !hamburger.contains(e.target)) sidebar.classList.remove('open');
    });
  }
}

window.renderSidebar = renderSidebar;
