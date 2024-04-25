import { defineConfig } from 'vitepress'
import { withMermaid } from "vitepress-plugin-mermaid"

// https://vitepress.dev/reference/site-config
export default defineConfig({
  base: '/',
  lang: 'en-AU',
  title: "Modeller.NET",
  description: "Code generation using BDD",
  themeConfig: {
    // https://vitepress.dev/reference/default-theme-config
    nav: [
      { text: 'Home', link: '/' },
      { text: 'Examples', link: '/markdown-examples' }
    ],

    sidebar: [
      {
        text: 'Examples',
        items: [
          { text: 'Markdown Examples', link: '/markdown-examples' },
          { text: 'Runtime API Examples', link: '/api-examples' }
        ]
      }
    ],

    socialLinks: [
      { icon: 'github', link: 'https://github.com/Modeller-net/Model.Engine' }
    ],

    footer: {
      message: 'Released under the MIT License.',
      copyright: 'Copyright © Allan Nielsen and contributors.',
    }
  }
})
