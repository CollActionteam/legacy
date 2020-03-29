export const siteData = {
  title: 'CollAction',
  author: 'CollAction',
  description: 'CollAction',
  mailchimpListId: '1a035c45ca',
  loginProviders: [
    {
      name: 'Google'
    },
    {
      name: 'Twitter'
    },
    {
      name: 'Facebook'
    }
  ],
  menuLinks: [
    {
      name: 'Home',
      link: '',
      showInPrimaryNavigation: true
    },
    {
      name: 'Find Project',
      link: '/projects/find',
      showInPrimaryNavigation: true
    },
    {
      name: 'Start Project',
      link: '/projects/start',
      showInPrimaryNavigation: true
    },
    {
      name: 'Blog',
      link: '/blogs',
      showInPrimaryNavigation: true
    },
    {
      name: 'About',
      link: '/about',
      showInPrimaryNavigation: true
    },
    {
      name: 'Login',
      link: '/account/login'
    },
    {
      name: 'Sign Up',
      link: '/account/register'
    },
    {
      name: 'Donate',
      link: '/donate'
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
      link: "/login",
      name: "Login",
    },
    {
      link: "/register-user",
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
      shareUrl: (url: string) => `https://www.facebook.com/sharer/sharer.php?u=${url}`,
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
      shareUrl: (url: string) => `https://twitter.com/intent/tweet?url=${url}`,
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
      shareUrl: (url: string) => `https://www.linkedin.com/shareArticle?mini=true&url=${url}&source=https%3A%2F%2Fcollaction.org`,
      icon: 'linkedin-in'
    }
  ]
};
