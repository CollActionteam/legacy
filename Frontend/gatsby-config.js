module.exports = {
  siteMetadata: {
    title: "CollAction",
    author: "CollAction",
    description: "CollAction",
    backendUrl: process.env.GATSBY_BACKEND_URL ? process.env.GATSBY_BACKEND_URL : "https://test.collaction.org",
    mailchimpListId: "1a035c45ca",
    loginProviders: [
      {
        name: "Google",
      },
      {
        name: "Twitter",
      },
      {
        name: "Facebook",
      },
    ],
    menuLinks: [
      {
        name: "Home",
        link: "",
        showInPrimaryNavigation: true,
      },
      {
        name: "Find Project",
        link: "/projects/find",
        showInPrimaryNavigation: true,
      },
      {
        name: "Start Project",
        link: "/projects/start",
        showInPrimaryNavigation: true,
      },
      {
        name: "Blog",
        link: "/blogs",
        showInPrimaryNavigation: true,
      },
      {
        name: "About",
        link: "/about",
        showInPrimaryNavigation: true,
      },
      {
        name: "Login",
        link: "/account/login",
      },
      {
        name: "Sign Up",
        link: "/account/register",
      },
      {
        name: "Donate",
        link: "/donate",
      },
    ],
    socialMedia: [
      {
        name: "Facebook",
        url: "https://www.facebook.com/collaction.org",
        icon: "facebook-f",
      },
      {
        name: "Instagram",
        url: "https://www.instagram.com/collaction_org",
        icon: "instagram",
      },
      {
        name: "Twitter",
        url: "https://twitter.com/CollAction_org",
        icon: "twitter",
      },
      {
        name: "YouTube",
        url: "https://www.youtube.com/channel/UCC2SBF4mbeKXrHqnMuN6Iew",
        icon: "youtube",
      },
      {
        name: "LinkedIn",
        url: "https://www.linkedin.com/company-beta/15079855",
        icon: "linkedin-in",
      },
    ]
  },
  plugins: [
    {
      resolve: "gatsby-plugin-sass",
      options: {
        cssLoaderOptions: {
          camelCase: false,
        },
      },
    },
    {
      resolve: "gatsby-plugin-google-fonts",
      options: {
        fonts: ["Raleway\:300,400,500,600,700"],
        display: "swap",
      },
    },
    {
      resolve: 'gatsby-plugin-purgecss',
      options: {
        ignore: [
          '/node_modules/slick-carousel/slick/slick.css',
          '/node_modules/slick-carousel/slick/slick-theme.css'
        ]
      }
    },
    {
      resolve: "gatsby-source-filesystem",
      options: {
        path: `${__dirname}/src/content/home`,
        name: "home",
      },
    },
    {
      resolve: "gatsby-source-filesystem",
      options: {
        path: `${__dirname}/src/content/crowdactingsteps`,
        name: "crowdactingsteps",
      },
    },
    {
      resolve: "gatsby-source-filesystem",
      options: {
        path: `${__dirname}/src/content/findproject`,
        name: "findproject",
      },
    },
    {
      resolve: "gatsby-source-filesystem",
      options: {
        path: `${__dirname}/src/content/startproject`,
        name: "startproject",
      },
    },
    {
      resolve: "gatsby-source-filesystem",
      options: {
        path: `${__dirname}/src/content/thankyoucreate`,
        name: "thankyoucreate",
      },
    },
    {
      resolve: "gatsby-source-filesystem",
      options: {
        path: `${__dirname}/src/content/blogs`,
        name: "blogs",
      },
    },
    {
      resolve: "gatsby-source-filesystem",
      options: {
        path: `${__dirname}/src/content/about`,
        name: "about",
      },
    },
    {
      resolve: "gatsby-source-filesystem",
      options: {
        path: `${__dirname}/static/assets`,
        name: "assets",
      },
    },
    {
      resolve: "gatsby-transformer-remark",
      options: {
        plugins: [
          {
            resolve: "gatsby-remark-images",
            options: {
              maxWidth: 590,
            },
          },
          {
            resolve: "gatsby-remark-responsive-iframe",
            options: {
              wrapperStyle: "margin-bottom: 1.0725rem",
            },
          },
          "gatsby-remark-prismjs",
          "gatsby-remark-copy-linked-files",
          "gatsby-remark-smartypants",
        ],
      },
    },
    "gatsby-transformer-yaml",
    "gatsby-transformer-sharp",
    "gatsby-plugin-sharp",
    "gatsby-plugin-offline",
    "gatsby-plugin-react-helmet",
    "gatsby-plugin-netlify-cms",
    "gatsby-plugin-typescript",
    "gatsby-plugin-tslint"
  ],
};
