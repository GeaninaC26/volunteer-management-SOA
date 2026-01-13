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
      name: "scheduling",
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
  base: '/dist/scheduling/',
  server: {
    port: 5003,
  },
  preview: {
    port: 5003,
  },
})
