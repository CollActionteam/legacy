module.exports = {
  purge: ['./pages/**/*.{js,ts,jsx,tsx}', './components/**/*.{js,ts,jsx,tsx}'],
  darkMode: false, // or 'media' or 'class'
  theme: {
    container: {
      padding: {
        DEFAULT: '1rem',
        sm: '2rem',
        md: '7rem',
        lg: '16rem',
      },
    },
    extend: {
      colors: {
        white: {
          DEFAULT: '#F9F9F9'
        },
        collaction: {
          light: '#2EB494',
          DEFAULT: '#2EB494',
          dark: '#2EB494'
        },
        black: {
          0: '#EFEFEF',
          400: '#333',
        }
      }
    }
  },
  variants: {
    extend: {},
  },
  plugins: [],
}
