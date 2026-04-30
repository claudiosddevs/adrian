/**
 * data.js — Simulated backend data layer
 * Mirrors the FIS database schema: CLIENTES, CONTRATOS, PAGOS, RECLAMOS, PLANES, USUARIOS, BITACORA
 * In production, replace fetch() calls with real API endpoints at https://localhost:7001
 */

// ─── SEEDED DATA (matches the API seeder from the .NET backend) ───────────────

const DB = {
  usuarios: [
    { id: 1, username: 'admin',    password: 'Admin123*',    nombre: 'Administrador Sistema', rol: 'Administrador', activo: true },
    { id: 2, username: 'cajero1',  password: 'Cajero123*',   nombre: 'Carlos Mendoza',        rol: 'Cajero',        activo: true },
    { id: 3, username: 'tecnico1', password: 'Tecnico123*',  nombre: 'Luis Torrez',           rol: 'Tecnico',       activo: true },
    { id: 4, username: 'cliente1', password: 'Cliente123*',  nombre: 'María García',          rol: 'Cliente',       activo: true, clienteId: 1 },
    { id: 5, username: 'cliente2', password: 'Cliente123*',  nombre: 'Juan Pérez',            rol: 'Cliente',       activo: true, clienteId: 2 },
    { id: 6, username: 'cliente3', password: 'Cliente123*',  nombre: 'Ana Rodríguez',         rol: 'Cliente',       activo: true, clienteId: 3 },
  ],

  planes: [
    { id: 1, nombre: 'Básico 10 Mbps',  velocidad: '10 Mbps',  precio: 89.00,  tipo: 'Internet',  descripcion: 'Ideal para uso básico, redes sociales y correo.', activo: true },
    { id: 2, nombre: 'Hogar 25 Mbps',   velocidad: '25 Mbps',  precio: 139.00, tipo: 'Internet',  descripcion: 'Perfecto para streaming HD y trabajo desde casa.',  activo: true },
    { id: 3, nombre: 'Pro 50 Mbps',     velocidad: '50 Mbps',  precio: 199.00, tipo: 'Fibra',     descripcion: 'Fibra óptica para múltiples dispositivos y gaming.', activo: true },
    { id: 4, nombre: 'Ultra 100 Mbps',  velocidad: '100 Mbps', precio: 299.00, tipo: 'Fibra',     descripcion: 'Máxima velocidad para empresas y familias grandes.', activo: true },
    { id: 5, nombre: 'Hosting Básico',  velocidad: '—',        precio: 59.00,  tipo: 'Hosting',   descripcion: 'Hosting web con 5 GB de almacenamiento.',           activo: true },
    { id: 6, nombre: 'Dominio .bo',     velocidad: '—',        precio: 79.00,  tipo: 'Dominio',   descripcion: 'Registro de dominio .bo por 1 año.',                activo: true },
    { id: 7, nombre: 'Fibra 200 Mbps',  velocidad: '200 Mbps', precio: 449.00, tipo: 'Fibra',     descripcion: 'Para empresas exigentes con alta demanda.',         activo: true },
    { id: 8, nombre: 'Empresarial 500', velocidad: '500 Mbps', precio: 799.00, tipo: 'Fibra',     descripcion: 'Solución completa para grandes empresas.',          activo: true },
  ],

  clientes: [
    { id: 1, nombre: 'María García',     ci: '12345678', telefono: '78901234', email: 'maria.garcia@email.com',   direccion: 'Av. Camacho 123, La Paz',    tipo: 'Natural',   activo: true },
    { id: 2, nombre: 'Juan Pérez',       ci: '87654321', telefono: '76543210', email: 'juan.perez@email.com',     direccion: 'Calle Comercio 456, La Paz', tipo: 'Natural',   activo: true },
    { id: 3, nombre: 'Ana Rodríguez',    ci: '11223344', telefono: '71234567', email: 'ana.rodriguez@email.com',  direccion: 'Zona Sur, Calacoto, La Paz', tipo: 'Natural',   activo: true },
    { id: 4, nombre: 'TechBolivia S.R.L.',nit:'1234567890',telefono:'22334455',email:'info@techbolivia.com',      direccion: 'Sopocachi, La Paz',          tipo: 'Juridico',  activo: true },
    { id: 5, nombre: 'Roberto Mamani',   ci: '55667788', telefono: '79876543', email: 'r.mamani@email.com',       direccion: 'El Alto, La Paz',            tipo: 'Natural',   activo: true },
  ],

  contratos: [
    { id: 1, clienteId: 1, planId: 2, fechaInicio: '2024-01-15', fechaFin: '2025-01-14', estado: 'Activo',    montoMensual: 139.00, nroContrato: 'CON-2024-001' },
    { id: 2, clienteId: 2, planId: 3, fechaInicio: '2024-02-01', fechaFin: '2025-01-31', estado: 'Activo',    montoMensual: 199.00, nroContrato: 'CON-2024-002' },
    { id: 3, clienteId: 3, planId: 1, fechaInicio: '2024-03-10', fechaFin: '2025-03-09', estado: 'Activo',    montoMensual: 89.00,  nroContrato: 'CON-2024-003' },
    { id: 4, clienteId: 4, planId: 7, fechaInicio: '2024-01-01', fechaFin: '2024-12-31', estado: 'Vencido',   montoMensual: 449.00, nroContrato: 'CON-2024-004' },
    { id: 5, clienteId: 5, planId: 2, fechaInicio: '2024-06-01', fechaFin: '2025-05-31', estado: 'Activo',    montoMensual: 139.00, nroContrato: 'CON-2024-005' },
  ],

  pagos: [
    { id:  1, contratoId: 1, clienteId: 1, monto: 139.00, fecha: '2024-12-15', mes: 'Diciembre 2024', metodo: 'Transferencia', estado: 'Pagado',  recibo: 'REC-001' },
    { id:  2, contratoId: 1, clienteId: 1, monto: 139.00, fecha: '2025-01-15', mes: 'Enero 2025',     metodo: 'Efectivo',      estado: 'Pagado',  recibo: 'REC-002' },
    { id:  3, contratoId: 1, clienteId: 1, monto: 139.00, fecha: null,         mes: 'Febrero 2025',   metodo: null,            estado: 'Pendiente', recibo: null },
    { id:  4, contratoId: 2, clienteId: 2, monto: 199.00, fecha: '2025-01-10', mes: 'Enero 2025',     metodo: 'QR',            estado: 'Pagado',  recibo: 'REC-003' },
    { id:  5, contratoId: 2, clienteId: 2, monto: 199.00, fecha: null,         mes: 'Febrero 2025',   metodo: null,            estado: 'Mora',    recibo: null },
    { id:  6, contratoId: 3, clienteId: 3, monto:  89.00, fecha: '2025-01-20', mes: 'Enero 2025',     metodo: 'Efectivo',      estado: 'Pagado',  recibo: 'REC-004' },
    { id:  7, contratoId: 3, clienteId: 3, monto:  89.00, fecha: null,         mes: 'Febrero 2025',   metodo: null,            estado: 'Pendiente', recibo: null },
    { id:  8, contratoId: 5, clienteId: 5, monto: 139.00, fecha: '2025-01-05', mes: 'Enero 2025',     metodo: 'Tarjeta',       estado: 'Pagado',  recibo: 'REC-005' },
    { id:  9, contratoId: 5, clienteId: 5, monto: 139.00, fecha: null,         mes: 'Febrero 2025',   metodo: null,            estado: 'Mora',    recibo: null },
  ],

  reclamos: [
    { id: 1, clienteId: 1, tecnicoId: 3, titulo: 'Sin conexión a internet', descripcion: 'Desde ayer no tengo conexión, el router muestra luz roja.', tipo: 'Leve',    estado: 'Resuelto',    fecha: '2025-01-10', calificacion: 5, observacion: 'Se reinició el equipo ONU.' },
    { id: 2, clienteId: 2, tecnicoId: 3, titulo: 'Velocidad muy lenta',     descripcion: 'La velocidad bajó de 50 a 5 Mbps en los últimos días.',   tipo: 'Medio',   estado: 'En Proceso',  fecha: '2025-02-05', calificacion: null, observacion: null },
    { id: 3, clienteId: 3, tecnicoId: null, titulo: 'Router dañado',         descripcion: 'El router se apagó y no enciende más.',                    tipo: 'Complejo',estado: 'Recibido',    fecha: '2025-02-10', calificacion: null, observacion: null },
    { id: 4, clienteId: 1, tecnicoId: null, titulo: 'Error en factura',      descripcion: 'Me cobraron doble este mes.',                              tipo: 'Leve',    estado: 'En Proceso',  fecha: '2025-02-12', calificacion: null, observacion: null },
    { id: 5, clienteId: 5, tecnicoId: null, titulo: 'Corte de servicio',     descripcion: 'Me cortaron el servicio pero tengo el pago al día.',       tipo: 'Medio',   estado: 'Recibido',    fecha: '2025-02-14', calificacion: null, observacion: null },
  ],

  bitacora: [
    { id: 1, usuarioId: 1, accion: 'LOGIN',          descripcion: 'Ingreso al sistema', fecha: '2025-02-15T08:00:00', entidad: 'Sistema' },
    { id: 2, usuarioId: 2, accion: 'PAGO_REGISTRADO',descripcion: 'Pago REC-005 registrado', fecha: '2025-02-14T14:30:00', entidad: 'Pagos' },
    { id: 3, usuarioId: 3, accion: 'RECLAMO_ATENDIDO',descripcion:'Reclamo #2 actualizado a En Proceso', fecha: '2025-02-13T10:15:00', entidad: 'Reclamos' },
  ],

  // Next IDs for simulated inserts
  _nextIds: { pagos: 10, reclamos: 6, bitacora: 4 }
};

// ─── SESSION MANAGEMENT ───────────────────────────────────────────────────────

const Auth = {
  login(username, password) {
    const user = DB.usuarios.find(u => u.username === username && u.password === password && u.activo);
    if (!user) return null;
    const token = btoa(JSON.stringify({ userId: user.id, rol: user.rol, exp: Date.now() + 3600000 }));
    localStorage.setItem('fis_token', token);
    localStorage.setItem('fis_user', JSON.stringify(user));
    // Log login
    DB.bitacora.push({ id: DB._nextIds.bitacora++, usuarioId: user.id, accion: 'LOGIN', descripcion: `${user.nombre} ingresó al sistema`, fecha: new Date().toISOString(), entidad: 'Sistema' });
    return user;
  },
  logout() {
    localStorage.removeItem('fis_token');
    localStorage.removeItem('fis_user');
    window.location.href = '../login.html';
  },
  currentUser() {
    const raw = localStorage.getItem('fis_user');
    return raw ? JSON.parse(raw) : null;
  },
  isLoggedIn() {
    const token = localStorage.getItem('fis_token');
    if (!token) return false;
    try {
      const payload = JSON.parse(atob(token));
      return payload.exp > Date.now();
    } catch { return false; }
  },
  requireAuth() {
    if (!this.isLoggedIn()) { window.location.href = '../login.html'; }
  },
  requireRole(...roles) {
    const user = this.currentUser();
    if (!user || !roles.includes(user.rol)) { window.location.href = '../login.html'; }
  }
};

// ─── DATA ACCESS LAYER ────────────────────────────────────────────────────────
// These functions simulate what the REST API endpoints return.
// Replace with fetch('https://localhost:7001/api/...') in production.

const API = {
  // Auth
  async login(username, password) {
    await delay(600);
    const user = Auth.login(username, password);
    if (!user) throw new Error('Credenciales incorrectas. Verifique su usuario y contraseña.');
    return user;
  },

  // Cliente endpoints
  async getClienteById(id) {
    await delay(200);
    return DB.clientes.find(c => c.id === id) || null;
  },
  async getAllClientes() {
    await delay(300);
    return [...DB.clientes];
  },
  async createCliente(data) {
    await delay(400);
    const nuevo = { id: DB.clientes.length + 1, ...data, activo: true };
    DB.clientes.push(nuevo);
    return nuevo;
  },
  async updateCliente(id, data) {
    await delay(300);
    const idx = DB.clientes.findIndex(c => c.id === id);
    if (idx === -1) throw new Error('Cliente no encontrado');
    DB.clientes[idx] = { ...DB.clientes[idx], ...data };
    return DB.clientes[idx];
  },

  // Planes
  async getAllPlanes() {
    await delay(200);
    return DB.planes.filter(p => p.activo);
  },
  async getPlanById(id) {
    await delay(100);
    return DB.planes.find(p => p.id === id) || null;
  },

  // Contratos
  async getContratosByCliente(clienteId) {
    await delay(250);
    return DB.contratos.filter(c => c.clienteId === clienteId).map(c => ({
      ...c,
      plan: DB.planes.find(p => p.id === c.planId),
      cliente: DB.clientes.find(cl => cl.id === c.clienteId)
    }));
  },
  async getAllContratos() {
    await delay(300);
    return DB.contratos.map(c => ({
      ...c,
      plan: DB.planes.find(p => p.id === c.planId),
      cliente: DB.clientes.find(cl => cl.id === c.clienteId)
    }));
  },

  // Pagos
  async getPagosByCliente(clienteId) {
    await delay(250);
    return DB.pagos.filter(p => p.clienteId === clienteId);
  },
  async getAllPagos() {
    await delay(300);
    return DB.pagos.map(p => ({
      ...p,
      cliente: DB.clientes.find(c => c.id === p.clienteId)
    }));
  },
  async registrarPago(contratoId, clienteId, mes, metodo, monto) {
    await delay(800);
    const pago = DB.pagos.find(p => p.clienteId === clienteId && p.mes === mes);
    if (pago) {
      pago.estado = 'Pagado';
      pago.fecha = new Date().toISOString().split('T')[0];
      pago.metodo = metodo;
      pago.recibo = `REC-${String(DB._nextIds.pagos).padStart(3,'0')}`;
      DB._nextIds.pagos++;
      return pago;
    }
    // New payment record
    const nuevo = { id: DB._nextIds.pagos++, contratoId, clienteId, monto, fecha: new Date().toISOString().split('T')[0], mes, metodo, estado: 'Pagado', recibo: `REC-${String(DB._nextIds.pagos).padStart(3,'0')}` };
    DB.pagos.push(nuevo);
    return nuevo;
  },
  async getMora() {
    await delay(200);
    // Mora: payments with estado='Mora' (payment past día 12 of month)
    return DB.pagos.filter(p => p.estado === 'Mora').map(p => ({
      ...p,
      cliente: DB.clientes.find(c => c.id === p.clienteId)
    }));
  },

  // Reclamos
  async getReclamosByCliente(clienteId) {
    await delay(250);
    return DB.reclamos.filter(r => r.clienteId === clienteId);
  },
  async getAllReclamos() {
    await delay(300);
    return DB.reclamos.map(r => ({
      ...r,
      cliente: DB.clientes.find(c => c.id === r.clienteId)
    }));
  },
  async crearReclamo(clienteId, titulo, descripcion, tipo) {
    await delay(500);
    const nuevo = { id: DB._nextIds.reclamos++, clienteId, tecnicoId: null, titulo, descripcion, tipo, estado: 'Recibido', fecha: new Date().toISOString().split('T')[0], calificacion: null, observacion: null };
    DB.reclamos.push(nuevo);
    return nuevo;
  },
  async updateReclamoEstado(id, estado, observacion) {
    await delay(300);
    const r = DB.reclamos.find(r => r.id === id);
    if (!r) throw new Error('Reclamo no encontrado');
    r.estado = estado;
    if (observacion) r.observacion = observacion;
    return r;
  },
  async asignarTecnico(reclamoId, tecnicoId) {
    await delay(300);
    const r = DB.reclamos.find(r => r.id === reclamoId);
    if (!r) throw new Error('Reclamo no encontrado');
    // Max 5 active reclamos per technician
    const activos = DB.reclamos.filter(x => x.tecnicoId === tecnicoId && x.estado !== 'Resuelto').length;
    if (activos >= 5) throw new Error('El técnico ya tiene 5 reclamos activos asignados.');
    r.tecnicoId = tecnicoId;
    r.estado = 'En Proceso';
    return r;
  },
  async calificarReclamo(id, calificacion) {
    await delay(300);
    const r = DB.reclamos.find(r => r.id === id);
    if (!r) throw new Error('Reclamo no encontrado');
    r.calificacion = calificacion;
    return r;
  },

  // Reportes
  async getReporteMora() {
    await delay(400);
    return this.getMora();
  },
  async getBitacora() {
    await delay(300);
    return [...DB.bitacora].reverse();
  },
  async getResumenDashboard(clienteId) {
    await delay(300);
    if (clienteId) {
      const contratos = DB.contratos.filter(c => c.clienteId === clienteId);
      const pagos = DB.pagos.filter(p => p.clienteId === clienteId);
      const reclamos = DB.reclamos.filter(r => r.clienteId === clienteId);
      const enMora = pagos.some(p => p.estado === 'Mora');
      return { contratos: contratos.length, pagos: pagos.filter(p=>p.estado==='Pagado').length, reclamos: reclamos.length, enMora };
    }
    return {
      totalClientes: DB.clientes.length,
      contratosActivos: DB.contratos.filter(c=>c.estado==='Activo').length,
      pagosMes: DB.pagos.filter(p=>p.estado==='Pagado').length,
      reclamosAbiertos: DB.reclamos.filter(r=>r.estado!=='Resuelto').length,
      enMora: DB.pagos.filter(p=>p.estado==='Mora').length,
    };
  }
};

// ─── UTILITY ──────────────────────────────────────────────────────────────────
function delay(ms) { return new Promise(r => setTimeout(r, ms)); }

function formatCurrency(amount) {
  return `Bs. ${parseFloat(amount).toFixed(2)}`;
}

function formatDate(dateStr) {
  if (!dateStr) return '—';
  const d = new Date(dateStr + 'T00:00:00');
  return d.toLocaleDateString('es-BO', { year: 'numeric', month: 'long', day: 'numeric' });
}

function showToast(message, type = 'success') {
  const existing = document.getElementById('fis-toast');
  if (existing) existing.remove();
  const toast = document.createElement('div');
  toast.id = 'fis-toast';
  toast.className = `toast toast--${type}`;
  toast.innerHTML = `<span class="toast__icon">${type === 'success' ? '✓' : type === 'error' ? '✕' : 'ℹ'}</span><span>${message}</span>`;
  document.body.appendChild(toast);
  requestAnimationFrame(() => toast.classList.add('toast--visible'));
  setTimeout(() => { toast.classList.remove('toast--visible'); setTimeout(() => toast.remove(), 300); }, 3500);
}

function setLoading(btn, loading) {
  if (loading) { btn.dataset.orig = btn.innerHTML; btn.innerHTML = '<span class="spinner"></span> Procesando...'; btn.disabled = true; }
  else { btn.innerHTML = btn.dataset.orig; btn.disabled = false; }
}

function estadoBadge(estado) {
  const map = {
    'Activo': 'badge--green', 'Pagado': 'badge--green', 'Resuelto': 'badge--green',
    'Pendiente': 'badge--yellow', 'Recibido': 'badge--yellow', 'En Proceso': 'badge--blue',
    'Mora': 'badge--red', 'Vencido': 'badge--red', 'Suspendido': 'badge--red',
  };
  return `<span class="badge ${map[estado] || 'badge--gray'}">${estado}</span>`;
}

function tipoBadge(tipo) {
  const map = { 'Leve': 'badge--green', 'Medio': 'badge--yellow', 'Complejo': 'badge--red' };
  return `<span class="badge ${map[tipo] || 'badge--gray'}">${tipo}</span>`;
}

function iconoTipo(tipo) {
  const icons = { 'Internet': '📡', 'Fibra': '⚡', 'Hosting': '🖥️', 'Dominio': '🌐' };
  return icons[tipo] || '📦';
}

// Expose globals
window.Auth = Auth;
window.API = API;
window.DB = DB;
window.formatCurrency = formatCurrency;
window.formatDate = formatDate;
window.showToast = showToast;
window.setLoading = setLoading;
window.estadoBadge = estadoBadge;
window.tipoBadge = tipoBadge;
window.iconoTipo = iconoTipo;
