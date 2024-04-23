
import type { DefaultTheme, UserConfig } from "vitepress"
import { withMermaid } from "vitepress-plugin-mermaid"

const config: UserConfig<DefaultTheme.Config> = {
  base: '/',
  lang: 'en-US',
  title: 'Modeller.NET',
  description: 'Code generation using BDD',
  head: [
    ['link', { rel: 'apple-touch-icon', type: 'image/png', size: "180x180", href: '/apple-touch-icon.png' }],
    ['link', { rel: 'icon', type: 'image/png', size: "32x32", href: '/favicon-32x32.png' }],
    ['link', { rel: 'icon', type: 'image/png', size: "16x16", href: '/favicon-16x16.png' }],
    ['link', { rel: 'manifest', manifest: '/manifest.json' }],
    ['meta', { property: 'og:title', content: 'Modeller.NET' }],
    ['meta', { property: 'og:type', content: 'website' }],
    ['meta', { property: 'og:description', content: 'Code generation using BDD' }],
    ['meta', { property: 'og:image', content: 'https://modeller.net/social.png' }],
    ['meta', { property: 'og:url', content: 'https://modeller.net' }]
  ],

  lastUpdated: true,

  themeConfig: {
    logo: '/logo.png',

    nav: [
      {
        text: 'latest (v3.x)',
        items: [
          { text: 'v2.x', link: 'https://modeller-docs-v2.netlify.app', target: "_blank" },
          { text: 'v1.x', link: 'https://modeller-docs-v1.netlify.app', target: "_blank" },
        ]
      },
      { text: 'Intro', link: '/introduction' }
    ],

    search: {
      provider: 'local'
    },

    editLink: {
      pattern: 'https://github.com/Modeller-net/Model.Engine/edit/master/docs/:path',
      text: 'Suggest changes to this page'
    },

    socialLinks: [
      { icon: 'github', link: 'https://github.com/Modeller-net/Model.Engine' },
    ],

    footer: {
      message: 'Released under the MIT License.',
      copyright: 'Copyright © Allan Nielsen and contributors.',
    },

    sidebar: {
      '/': [
        {
          text: 'Introduction',
          collapsed: false,
          items: [
            { text: 'What is Modeller.NET?', link: '/introduction' },
            { text: 'Getting Started', link: '/getting-started' }
          ]
        }
      ]
    }
  }
}

export default withMermaid(config)
