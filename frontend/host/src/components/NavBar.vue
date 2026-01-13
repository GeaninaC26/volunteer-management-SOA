<template>
  <!-- ======= Header ======= -->
  <header id="header" class="navbar">
    <div class="navbar-container">
      <!-- Left: Logo -->
      <a href="#hero" class="logo-link">
        <img
          src="../utils/logo.png"
          alt="Logo"
          class="logo-image"
        />
      </a>

      <!-- Center: Title -->
      <div class="title-container">
        <span class="navbar-title">
          Volunteer Management App
        </span>
      </div>

      <!-- Right: Navigation Links -->
      <nav class="nav-links">
        <router-link to="/campaigns" class="nav-link">
          Campaigns
        </router-link>
      </nav>
    </div>
  </header>
</template>

<style scoped>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

.navbar {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  height: 15%;
  width: 100%;
  background-color: brown;
  z-index: 1000;
  padding: 0;
}

.navbar-container {
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: 100%;
  width: 100%;
  position: relative;
  padding: 0;
}

.logo-link {
  height: 100%;
  display: flex;
  align-items: center;
  text-decoration: none;
}

.logo-image {
  height: 100%;
  width: auto;
  object-fit: fill;
  display: block;
  padding: 0;
  margin: 0;
  max-width: 100%;
}

.title-container {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  z-index: 1;
}

.navbar-title {
  color: white;
  font-size: 36px;
  font-weight: bold;
  text-align: center;
  text-shadow: 1px 1px 2px #000;
}

.nav-links {
  display: flex;
  gap: 2rem;
  align-items: center;
  padding-right: 2rem;
}

.nav-link {
  color: white;
  font-size: 36px;
  font-weight: bold;
  text-align: center;
  text-shadow: 1px 1px 2px #000;
  text-decoration: none;
  transition: opacity 0.3s ease;
}

.nav-link:hover {
  opacity: 0.8;
}

/* Body adjustment for navbar */
:global(body) {
  padding-top: 120px;
}
</style>
