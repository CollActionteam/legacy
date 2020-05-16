export const siteData = {
  title: 'CollAction',
  author: 'CollAction',
  description: 'CollAction',
  menuLinks: [
    {
      name: 'Home',
      link: '/'
    },
    {
      name: 'Find Crowdaction',
      link: '/crowdactions/find'
    },
    {
      name: 'Start Crowdaction',
      link: '/crowdactions/start'
    },
    {
      name: 'About',
      link: '/about'
    }
  ],
  footerLinks: [
    {
      title: 'Explore',
      links: [
        {
          link: "/",
          name: "Home",
        },
        {
          link: "/crowdactions/find",
          name: "Find Crowdaction",
        },
        {
          link: "/crowdactions/start",
          name: "Start Crowdaction",
        },
        {
          link: "/account/login",
          name: "Login"
        },
        {
          link: "/account/register-user",
          name: "Register",
        },
      ]
    },
    {
      title: 'About us',
      links: [
        {
          link: "/about#mission",
          name: "Mission",
        },
        {
          link: "/about#team",
          name: "Team",
        },
        {
          link: "/about#partners",
          name: "Partners",
        },
        {
          link: "/donate",
          name: "Donate",
        }
      ],
    },
    {
      title: 'Support',
      links: [
        {
          link: "/about#faq",
          name: "FAQs",
        },
        {
          link: "/privacy-policy",
          name: "Privacy policy",
        },
      ]
    }
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
      shareUrl: (url: string, text?: string) => `https://twitter.com/intent/tweet?url=${encodeURIComponent(url)}` +
          (text ? `&text=${encodeURIComponent(text)}` : ''),
      icon: 'twitter'
    },
    {
      name: 'YouTube',
      url: 'https://www.youtube.com/channel/UCC2SBF4mbeKXrHqnMuN6Iew',
      icon: 'youtube'
    },
    {
      name: 'LinkedIn',
      url: 'https://www.linkedin.com/company/stichting-collaction/',
      shareUrl: (url: string) => `https://www.linkedin.com/shareArticle?mini=true&url=${encodeURIComponent(url)}&source=https%3A%2F%2Fcollaction.org`,
      icon: 'linkedin-in'
    }
  ]
};
