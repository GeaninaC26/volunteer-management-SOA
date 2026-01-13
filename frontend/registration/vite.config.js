import { fileURLToPath, URL } from 'node:url'
import { federation } from '@module-federation/vite';

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    vueDevTools(),
    federation({
      name: "registration",
      filename: "remoteEntry.js",
      exposes: {
        "./App": "./src/App.vue",
      },
      shared: ["vue", "vue-router"],
    }),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    },
  },
  base: '/dist/registration/',
  server: {
    port: 5002,
  },
  preview: {
    port: 5002,
  },
})
