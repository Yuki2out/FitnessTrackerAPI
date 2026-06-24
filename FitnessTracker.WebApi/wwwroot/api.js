
const API_BASE = "http:


const Auth = {
  getToken() {
    return localStorage.getItem("ft_token");
  },
  getUser() {
    const raw = localStorage.getItem("ft_user");
    return raw ? JSON.parse(raw) : null;
  },
  setSession(authResponse) {
    localStorage.setItem("ft_token", authResponse.token);
    localStorage.setItem(
      "ft_user",
      JSON.stringify({
        email: authResponse.email,
        fullName: authResponse.fullName,
        roles: authResponse.roles || [],
      })
    );
  },
  clearSession() {
    localStorage.removeItem("ft_token");
    localStorage.removeItem("ft_user");
  },
  isLoggedIn() {
    return !!this.getToken();
  },
  isAdmin() {
    const user = this.getUser();
    return !!user && user.roles.includes("Administrator");
  },
  requireAuth() {
    if (!this.isLoggedIn()) {
      window.location.href = "index.html";
    }
  },
  logout() {
    this.clearSession();
    window.location.href = "index.html";
  },
};
 




async function apiFetch(path, { method = "GET", body, auth = true } = {}) {
  const headers = { "Content-Type": "application/json" };
 
  if (auth) {
    const token = Auth.getToken();
    if (token) headers["Authorization"] = `Bearer ${token}`;
  }
 
  const response = await fetch(`${API_BASE}${path}`, {
    method,
    headers,
    body: body !== undefined ? JSON.stringify(body) : undefined,
  });
 
  
  if (response.status === 204) return null;
 
  let data = null;
  const text = await response.text();
  if (text) {
    try {
      data = JSON.parse(text);
    } catch {
      data = text;
    }
  }
 
  if (!response.ok) {
    let message = `Request failed (${response.status})`;
 
    if (data) {
      if (typeof data === "string") {
        message = data;
      } else if (data.message) {
        message = data.message;
      } else if (data.errors) {
        
        const allMessages = Object.values(data.errors).flat();
        if (allMessages.length) message = allMessages.join(" ");
      }
    }
 
    if (response.status === 401) {
      message = "Your session has expired. Please log in again.";
      Auth.clearSession();
    }
 
    const err = new Error(message);
    err.status = response.status;
    err.data = data;
    throw err;
  }
 
  return data;
}
 


const MUSCLE_GROUPS = ["Chest", "Legs", "Back", "Shoulders", "Arms", "Core"];
 
function muscleGroupLabel(value) {
  if (typeof value === "number") return MUSCLE_GROUPS[value] ?? "Unknown";
  return value; 
}
 

function renderNav(activePage) {
  const root = document.getElementById("nav-root");
  if (!root) return;
 
  const user = Auth.getUser();
  const links = [

    { href: "workout.html", label: "Workout", key: "workout" },
    { href: "dashboard.html", label: "Dashboard", key: "dashboard" },
    { href: "exercises.html", label: "Exercises", key: "exercises" },
    { href: "library.html", label: "Library", key: "labrary" }
  ];
 
  const linksHtml = links
    .map(
      (l) =>
        `<a href="${l.href}" class="${l.key === activePage ? "active" : ""}">${l.label}</a>`
    )
    .join("");
 
  root.innerHTML = `
    <div class="topbar">
      <div class="topbar-inner">
        <a href="dashboard.html" class="brand"><span class="brand-mark"></span>FITTRACK</a>
        <div class="nav-links">
          ${linksHtml}
          <span class="nav-user">${ "   "+user ? user.fullName : ""}</span>
          <button class="btn-logout" id="logout-btn">Log out</button>
        </div>
      </div>
    </div>
  `;
 
  document.getElementById("logout-btn").addEventListener("click", () => Auth.logout());
}
 

function formatDate(isoString) {
  const d = new Date(isoString);
  return d.toLocaleDateString(undefined, {
    year: "numeric",
    month: "short",
    day: "numeric",
  });
}
 
function showAlert(el, message, type = "error") {
  el.textContent = message;
  el.classList.remove("hidden", "alert-error", "alert-success");
  el.classList.add(type === "error" ? "alert-error" : "alert-success");
}
 
function hideAlert(el) {
  el.classList.add("hidden");
  el.textContent = "";
}
