
const AGRAVOS = [
  { nome: "Trauma",       pct: 28.4, cor: "#E63946" },
  { nome: "Clínico",      pct: 24.7, cor: "#F4A3AC" },
  { nome: "Cardiológico", pct: 12.3, cor: "#3B6FE0" },
  { nome: "Respiratório", pct: 10.6, cor: "#5B8DEF" },
  { nome: "Psiquiátrico", pct: 8.9,  cor: "#F59E0B" },
  { nome: "Outros",       pct: 15.1, cor: "#2BB39A" },
];

const BAIRROS = [
  { nome: "Boa Viagem",    total: 6120 },
  { nome: "Imbiribeira",   total: 4380 },
  { nome: "Casa Amarela",  total: 3910 },
  { nome: "Afogados",      total: 3240 },
  { nome: "Iputinga",      total: 2150 },
  { nome: "Várzea",        total: 1780 },
  { nome: "Torre",         total: 1120 },
  { nome: "Boa Vista",     total: 860 },
  { nome: "Espinheiro",    total: 420 },
];

const GRAVIDADES = ["Alta", "Média", "Baixa"];

let logs = [
  { hora: "27/10 14:21", msg: "ETL concluído com sucesso",   tipo: "ok" },
  { hora: "27/10 14:17", msg: "Remoção de duplicados",       tipo: "ok" },
  { hora: "27/10 14:03", msg: "Validação de estrutura",      tipo: "ok" },
  { hora: "27/10 13:58", msg: "Início do processamento",     tipo: "info" },
  { hora: "27/10 13:44", msg: "Leitura de planilhas .xlsx",  tipo: "info" },
];


function gerarOcorrencias(qtd) {
  const lista = [];
  let data = new Date(2025, 9, 27, 14, 21);
  for (let i = 0; i < qtd; i++) {
    lista.push({
      id: String(24567 - i).padStart(8, "0"),
      data: new Date(data),
      bairro: BAIRROS[Math.floor(Math.random() * BAIRROS.length)].nome,
      tipo: AGRAVOS[Math.floor(Math.random() * AGRAVOS.length)].nome,
      gravidade: GRAVIDADES[Math.floor(Math.random() * 3)],
    });
    data = new Date(data.getTime() - (4 + Math.random() * 14) * 60000);
  }
  return lista;
}
const ocorrencias = gerarOcorrencias(60);


const serieDiaria = Array.from({ length: 30 }, (_, i) => {
  const d = new Date(2025, 8, 28 + i);
  const base = 4800 + Math.sin(i / 2.5) * 600 + i * 30;
  return { data: d, valor: Math.round(base + (Math.random() - 0.5) * 900) };
});


const $ = (s) => document.querySelector(s);
const $$ = (s) => document.querySelectorAll(s);
const fmt = (n) => n.toLocaleString("pt-BR");
const pad = (n) => String(n).padStart(2, "0");
const fmtDataHora = (d) => `${pad(d.getDate())}/${pad(d.getMonth() + 1)} ${pad(d.getHours())}:${pad(d.getMinutes())}`;
const fmtDataCompleta = (d) => `${pad(d.getDate())}/${pad(d.getMonth() + 1)}/${d.getFullYear()} ${pad(d.getHours())}:${pad(d.getMinutes())}`;

function toast(msg) {
  const t = $("#toast");
  t.textContent = msg;
  t.classList.add("show");
  clearTimeout(t._timer);
  t._timer = setTimeout(() => t.classList.remove("show"), 2800);
}


const TITULOS = {
  etl: "ETL / Processamento",
  base: "Base Histórica",
  relatorios: "Relatórios",
  config: "Configurações",
};

function irPara(view) {
  $$(".nav-item").forEach((a) => a.classList.toggle("active", a.dataset.view === view));
  $$(".view").forEach((v) => v.classList.remove("active"));

  const alvo = $(`#view-${view}`);
  if (alvo) {
    alvo.classList.add("active");
  } else {
    $("#placeholderTitle").textContent = TITULOS[view] || "Em construção";
    $("#view-placeholder").classList.add("active");
  }

  if (view === "inicio") desenharGraficos();
  fecharMenu();
  window.scrollTo({ top: 0, behavior: "smooth" });
}

$$(".nav-item").forEach((a) =>
  a.addEventListener("click", (e) => { e.preventDefault(); irPara(a.dataset.view); })
);
$$("[data-goto]").forEach((a) =>
  a.addEventListener("click", (e) => { e.preventDefault(); irPara(a.dataset.goto); })
);


function fecharMenu() {
  $("#sidebar").classList.remove("open");
  $("#overlay").classList.remove("show");
}
$("#menuBtn").addEventListener("click", () => {
  $("#sidebar").classList.add("open");
  $("#overlay").classList.add("show");
});
$("#overlay").addEventListener("click", fecharMenu);


$("#bellBtn").addEventListener("click", (e) => {
  e.stopPropagation();
  $("#notifPanel").classList.toggle("open");
  $("#bellBtn .dot").style.display = "none";
});
document.addEventListener("click", (e) => {
  if (!$("#notifPanel").contains(e.target)) $("#notifPanel").classList.remove("open");
});


function animarContadores() {
  $$(".kpi-value").forEach((el) => {
    const alvo = Number(el.dataset.count);
    const sufixo = el.dataset.suffix || "";
    const inicio = performance.now();
    const dur = 1200;
    function passo(agora) {
      const p = Math.min((agora - inicio) / dur, 1);
      const eased = 1 - Math.pow(1 - p, 3);
      el.textContent = fmt(Math.round(alvo * eased)) + sufixo;
      if (p < 1) requestAnimationFrame(passo);
    }
    requestAnimationFrame(passo);
  });
}


const lineCanvas = $("#lineChart");
let pontosLinha = [];

function desenharLinha() {
  const ctx = lineCanvas.getContext("2d");
  const dpr = window.devicePixelRatio || 1;
  const w = lineCanvas.clientWidth;
  const h = lineCanvas.clientHeight;
  if (!w) return;
  lineCanvas.width = w * dpr;
  lineCanvas.height = h * dpr;
  ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
  ctx.clearRect(0, 0, w, h);

  const pad = { t: 10, r: 10, b: 28, l: 44 };
  const max = 8000;
  const cw = w - pad.l - pad.r;
  const ch = h - pad.t - pad.b;

  
  ctx.font = "11px Inter, sans-serif";
  ctx.fillStyle = "#94A3B8";
  ctx.strokeStyle = "rgba(241,245,249,0.07)";
  ctx.lineWidth = 1;
  for (let v = 0; v <= max; v += 2000) {
    const y = pad.t + ch - (v / max) * ch;
    ctx.beginPath();
    ctx.moveTo(pad.l, y);
    ctx.lineTo(w - pad.r, y);
    ctx.stroke();
    ctx.textAlign = "right";
    ctx.fillText(fmt(v), pad.l - 8, y + 4);
  }

  
  pontosLinha = serieDiaria.map((d, i) => ({
    x: pad.l + (i / (serieDiaria.length - 1)) * cw,
    y: pad.t + ch - (d.valor / max) * ch,
    ...d,
  }));

  
  ctx.textAlign = "center";
  pontosLinha.forEach((p, i) => {
    if (i % 4 === 0 || i === pontosLinha.length - 1) {
      ctx.fillText(`${pad2(p.data.getDate())}/${pad2(p.data.getMonth() + 1)}`, p.x, h - 8);
    }
  });

  
  const grad = ctx.createLinearGradient(0, pad.t, 0, pad.t + ch);
  grad.addColorStop(0, "rgba(230,57,70,0.45)");
  grad.addColorStop(1, "rgba(230,57,70,0.02)");
  ctx.beginPath();
  ctx.moveTo(pontosLinha[0].x, pad.t + ch);
  pontosLinha.forEach((p) => ctx.lineTo(p.x, p.y));
  ctx.lineTo(pontosLinha.at(-1).x, pad.t + ch);
  ctx.closePath();
  ctx.fillStyle = grad;
  ctx.fill();

  
  ctx.beginPath();
  pontosLinha.forEach((p, i) => (i ? ctx.lineTo(p.x, p.y) : ctx.moveTo(p.x, p.y)));
  ctx.strokeStyle = "#E63946";
  ctx.lineWidth = 2;
  ctx.lineJoin = "round";
  ctx.stroke();

  
  if (desenharLinha.hover != null) {
    const p = pontosLinha[desenharLinha.hover];
    ctx.strokeStyle = "rgba(241,245,249,0.25)";
    ctx.setLineDash([4, 4]);
    ctx.beginPath();
    ctx.moveTo(p.x, pad.t);
    ctx.lineTo(p.x, pad.t + ch);
    ctx.stroke();
    ctx.setLineDash([]);
    ctx.beginPath();
    ctx.arc(p.x, p.y, 5, 0, Math.PI * 2);
    ctx.fillStyle = "#E63946";
    ctx.fill();
    ctx.strokeStyle = "#fff";
    ctx.lineWidth = 2;
    ctx.stroke();
  }
}
const pad2 = pad;

lineCanvas.addEventListener("mousemove", (e) => {
  const r = lineCanvas.getBoundingClientRect();
  const x = e.clientX - r.left;
  let idx = 0, menor = Infinity;
  pontosLinha.forEach((p, i) => {
    const d = Math.abs(p.x - x);
    if (d < menor) { menor = d; idx = i; }
  });
  desenharLinha.hover = idx;
  desenharLinha();
  const p = pontosLinha[idx];
  const tip = $("#lineTooltip");
  tip.innerHTML = `<b>${fmt(p.valor)}</b> ocorrências<br><small>${pad(p.data.getDate())}/${pad(p.data.getMonth() + 1)}</small>`;
  tip.style.left = p.x + "px";
  tip.style.top = p.y + "px";
  tip.style.opacity = 1;
});
lineCanvas.addEventListener("mouseleave", () => {
  desenharLinha.hover = null;
  desenharLinha();
  $("#lineTooltip").style.opacity = 0;
});


const donutCanvas = $("#donutChart");
let donutHover = -1;

function desenharDonut() {
  const ctx = donutCanvas.getContext("2d");
  const s = donutCanvas.width;
  const cx = s / 2, cy = s / 2;
  const raio = s / 2 - 8;
  const espessura = 30;
  ctx.clearRect(0, 0, s, s);

  let ang = -Math.PI / 2;
  AGRAVOS.forEach((a, i) => {
    const fatia = (a.pct / 100) * Math.PI * 2;
    const r = i === donutHover ? raio + 5 : raio;
    ctx.beginPath();
    ctx.arc(cx, cy, r, ang + 0.02, ang + fatia - 0.02);
    ctx.arc(cx, cy, r - espessura, ang + fatia - 0.02, ang + 0.02, true);
    ctx.closePath();
    ctx.fillStyle = a.cor;
    ctx.globalAlpha = donutHover === -1 || donutHover === i ? 1 : 0.35;
    ctx.fill();
    a._ini = ang;
    a._fim = ang + fatia;
    ang += fatia;
  });
  ctx.globalAlpha = 1;
}

function montarLegenda() {
  const ul = $("#donutLegend");
  ul.innerHTML = AGRAVOS.map(
    (a, i) => `<li data-i="${i}"><i style="background:${a.cor}"></i>${a.nome}<span>${a.pct.toFixed(1).replace(".", ",")}%</span></li>`
  ).join("");
  ul.querySelectorAll("li").forEach((li) => {
    li.addEventListener("mouseenter", () => destacarAgravo(Number(li.dataset.i)));
    li.addEventListener("mouseleave", () => destacarAgravo(-1));
  });
}

function destacarAgravo(i) {
  donutHover = i;
  desenharDonut();
  const ul = $("#donutLegend");
  ul.classList.toggle("hovering", i !== -1);
  ul.querySelectorAll("li").forEach((li) => li.classList.toggle("hl", Number(li.dataset.i) === i));
  const centro = $(".donut-center");
  if (i === -1) {
    centro.innerHTML = "<strong>238.764</strong><small>total</small>";
  } else {
    const a = AGRAVOS[i];
    centro.innerHTML = `<strong>${fmt(Math.round(238764 * a.pct / 100))}</strong><small>${a.nome}</small>`;
  }
}

donutCanvas.addEventListener("mousemove", (e) => {
  const r = donutCanvas.getBoundingClientRect();
  const escala = donutCanvas.width / r.width;
  const x = (e.clientX - r.left) * escala - donutCanvas.width / 2;
  const y = (e.clientY - r.top) * escala - donutCanvas.height / 2;
  const dist = Math.hypot(x, y);
  const raio = donutCanvas.width / 2 - 8;
  if (dist < raio - 30 || dist > raio + 6) return destacarAgravo(-1);
  let ang = Math.atan2(y, x);
  if (ang < -Math.PI / 2) ang += Math.PI * 2;
  const idx = AGRAVOS.findIndex((a) => ang >= a._ini && ang < a._fim);
  if (idx !== donutHover) destacarAgravo(idx);
});
donutCanvas.addEventListener("mouseleave", () => destacarAgravo(-1));


function corHeat(v) {
  if (v > 5000) return "var(--heat-5)";
  if (v > 2000) return "var(--heat-4)";
  if (v > 1000) return "var(--heat-3)";
  if (v > 500) return "var(--heat-2)";
  return "var(--heat-1)";
}

function montarBairros() {
  const max = Math.max(...BAIRROS.map((b) => b.total));
  $("#bairroList").innerHTML = BAIRROS.map(
    (b) => `<div class="bairro"><span>${b.nome}</span>
      <div class="bar"><span data-w="${(b.total / max) * 100}" style="background:${corHeat(b.total)}"></span></div>
      <b>${fmt(b.total)}</b></div>`
  ).join("");
  requestAnimationFrame(() =>
    $$("#bairroList .bar span").forEach((s) => (s.style.width = s.dataset.w + "%"))
  );
}


const linhaHTML = (o) => `<tr>
  <td>${o.id}</td>
  <td>${fmtDataHora(o.data)}</td>
  <td>${o.bairro}</td>
  <td>${o.tipo}</td>
  <td><span class="grav ${o.gravidade}">${o.gravidade}</span></td>
</tr>`;

function montarTabelaRecente() {
  $("#recentTable").innerHTML = ocorrencias.slice(0, 5).map(linhaHTML).join("");
}

function montarTabelaCompleta() {
  const termo = $("#searchInput").value.trim().toLowerCase();
  const tipo = $("#filterTipo").value;
  const grav = $("#filterGrav").value;

  const filtradas = ocorrencias.filter((o) => {
    const texto = `${o.id} ${o.bairro} ${o.tipo} ${o.gravidade}`.toLowerCase();
    return (!termo || texto.includes(termo)) &&
           (!tipo || o.tipo === tipo) &&
           (!grav || o.gravidade === grav);
  });

  $("#fullTable").innerHTML = filtradas.map(linhaHTML).join("");
  $("#emptyMsg").hidden = filtradas.length > 0;
}

$("#filterTipo").innerHTML += AGRAVOS.map((a) => `<option>${a.nome}</option>`).join("");
$("#filterTipo").addEventListener("change", montarTabelaCompleta);
$("#filterGrav").addEventListener("change", montarTabelaCompleta);

$("#searchInput").addEventListener("input", () => {
  if (!$("#view-ocorrencias").classList.contains("active")) irPara("ocorrencias");
  montarTabelaCompleta();
});


const corLog = { ok: "var(--green)", info: "var(--blue)", warn: "var(--red)" };
const logHTML = (l) => `<li><i style="background:${corLog[l.tipo]}"></i><time>${l.hora}</time><span>${l.msg}</span></li>`;

function montarLogs() {
  $("#recentLogs").innerHTML = logs.slice(0, 5).map(logHTML).join("");
  $("#allLogs").innerHTML = logs.map(logHTML).join("");
}

function addLog(msg, tipo = "info") {
  logs.unshift({ hora: fmtDataHora(new Date()), msg, tipo });
  montarLogs();
}


const ETAPAS = [
  { log: "Leitura de planilhas .xlsx", tempo: 900 },
  { log: "Validação de estrutura",     tempo: 1100 },
  { log: "Consolidação em base única", tempo: 1000 },
  { log: "Remoção de duplicados",      tempo: 1300 },
  { log: "ETL concluído com sucesso",  tempo: 700 },
];

$("#runPipeline").addEventListener("click", async () => {
  const btn = $("#runPipeline");
  const itens = $$("#pipeline li");
  btn.disabled = true;
  btn.textContent = "Executando...";
  itens.forEach((li) => { li.classList.remove("active"); li.classList.add("pending"); });

  const pill = $("#statusPill");
  const execPill = $("#execPill");
  [pill, execPill].forEach((p) => { p.className = "pill running"; p.textContent = "⏳ Em processamento"; });

  addLog("Início do processamento", "info");
  const inicio = Date.now();

  for (let i = 0; i < ETAPAS.length; i++) {
    itens[i].classList.remove("pending");
    itens[i].classList.add("active");
    await new Promise((r) => setTimeout(r, ETAPAS[i].tempo));
    itens[i].classList.remove("active");
    addLog(ETAPAS[i].log, "ok");
  }

  const agora = new Date();
  const registros = 240000 + Math.floor(Math.random() * 9000);
  const dup = 1800 + Math.floor(Math.random() * 600);
  const seg = ((Date.now() - inicio) / 1000).toFixed(1);

  $("#execDate").textContent = fmtDataCompleta(agora);
  $("#lastUpdate").textContent = fmtDataCompleta(agora);
  $("#execRecords").textContent = fmt(registros);
  $("#execDup").textContent = fmt(dup);
  $("#execTime").textContent = `${seg} s`;

  pill.className = "pill success";
  pill.textContent = "✓ Processamento concluído";
  execPill.className = "pill success";
  execPill.textContent = "✓ Concluído com sucesso";

  btn.disabled = false;
  btn.textContent = "▶ Executar pipeline";
  toast(`Pipeline finalizado: ${fmt(registros)} registros, ${fmt(dup)} duplicados removidos.`);
});


function desenharGraficos() {
  desenharLinha();
  desenharDonut();
}

let resizeTimer;
window.addEventListener("resize", () => {
  clearTimeout(resizeTimer);
  resizeTimer = setTimeout(desenharGraficos, 120);
});

document.addEventListener("DOMContentLoaded", () => {
  animarContadores();
  montarLegenda();
  montarBairros();
  montarTabelaRecente();
  montarTabelaCompleta();
  montarLogs();
  desenharGraficos();
});