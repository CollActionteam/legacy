export const siteData = {
  title: 'CollAction',
  author: 'CollAction',
  description: 'CollAction',
  menuLinks: [
    {
      name: 'Home',
      link: ''
    },
    {
      name: 'Find Project',
      link: '/projects/find'
    },
    {
      name: 'Start Project',
      link: '/projects/start'
    },
    {
      name: 'About',
      link: '/about'
    }
  ],
  footerLinks: [
    {
      link: "/",
      name: "Home",
    },
    {
      link: "/projects/find",
      name: "Find Project",
    },
    {
      link: "/projects/start",
      name: "Start Project",
    },
    {
      link: "/about",
      name: "About Us",
    },
    {
      link: "/account/login",
      name: "Login",
    },
    {
      link: "/account/register-user",
      name: "Sign Up",
    },
    {
      link: "/about#mission",
      name: "Mission",
    },
    {
      link: "/about#team",
      name: "Team",
    },
    {
      link: "/donate",
      name: "Donate",
    },
    {
      link: "/about#partners",
      name: "Partners",
    },
    {
      link: "/about#press",
      name: "Press",
    },
    {
      link: "/about#faq",
      name: "FAQs",
    },
  ],
  socialMedia: [
    {
      name: 'Facebook',
      url: 'https://www.facebook.com/collaction.org',
      shareUrl: (url: string) => `https://www.facebook.com/sharer/sharer.php?u=${encodeURIComponent(url)}`,
      icon: 'facebook-f'
    },
    {
      name: 'Instagram',
      url: 'https://www.instagram.com/collaction_org',
      icon: 'instagram'
    },
    {
      name: 'Twitter',
      url: 'https://twitter.com/CollAction_org',
      shareUrl: (url: string) => `https://twitter.com/intent/tweet?url=${encodeURIComponent(url)}`,
      icon: 'twitter'
    },
    {
      name: 'YouTube',
      url: 'https://www.youtube.com/channel/UCC2SBF4mbeKXrHqnMuN6Iew',
      icon: 'youtube'
    },
    {
      name: 'LinkedIn',
      url: 'https://www.linkedin.com/company-beta/15079855',
      shareUrl: (url: string) => `https://www.linkedin.com/shareArticle?mini=true&url=${encodeURIComponent(url)}&source=https%3A%2F%2Fcollaction.org`,
      icon: 'linkedin-in'
    }
  ]
};
