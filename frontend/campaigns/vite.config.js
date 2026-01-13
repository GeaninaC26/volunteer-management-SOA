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
      name: "campaigns",
      filename: "remoteEntry.js",
      exposes: {
        "./CampaignsView": "./src/views/CampaignsView.vue",
        "./RecruitingCampaignView": "./src/views/RecruitingCampaignView.vue",
      },
      shared: ["vue", "vue-router"],
    }),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    },
  },
  base: '/dist/campaigns/',
  server: {
    port: 5001,
  },
  preview: {
    port: 5001,
  },
})
