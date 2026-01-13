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
      name: 'host',
      remotes: {
        campaigns: {
          type: 'module',
          entry: '/dist/campaigns/remoteEntry.js',
          entryGlobalName: 'campaigns',
          shareScope: 'default'
        },
        registration: {
          type: 'module',
          entry: '/dist/registration/remoteEntry.js',
          entryGlobalName: 'registration',
          shareScope: 'default'
        },
        scheduling: {
          type: 'module',
          entry: '/dist/scheduling/remoteEntry.js',
          entryGlobalName: 'scheduling',
          shareScope: 'default'
        },
      },
      shared: ['vue', "vue-router"],
    }),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    },
  },
  base: '/',
  server: {
    port: 5000,
  },
  preview: {
    port: 5000,
  },
})
