import React from "react";
import Container from "@material-ui/core/Container";
import Grid from "@material-ui/core/Grid";
import { Link } from "gatsby";
import logo from "../../../static/assets/logo.svg";
import styles from "./style.module.scss";
import SocialMedia from "../SocialMedia";
import NewsletterSignup from "../NewsletterSignup";

const navigation = [
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
    link: "/signup",
    name: "Sign Up",
  },
  {
    link: "/about",
    name: "Mission",
  },
  {
    link: "/about",
    name: "Team",
  },
  {
    link: "/donate",
    name: "Donate",
  },
  {
    link: "/about",
    name: "Partners",
  },
  {
    link: "/about",
    name: "Press",
  },
  {
    link: "/about",
    name: "FAQs",
  },
];

export default () => (
  <div className={styles.footer}>
    <Container>
      <Link to="/">
        <img alt="CollAction" className={styles.logo} src={logo}></img>
      </Link>
      <Grid container>
        <Grid item xs={12} md={4}>
          <div className={styles.description}>
            Any questions, comments, or would you like to work together?
            Awesome! Email us at{" "}
            <Link to="mailto:collactionteam@gmail.com">
              collactionteam@gmail.com
            </Link>
            .<br />
            Would you like to get an occasional update on CollAction and
            crowdacting? Sign up for our newsletter (to your right)! We will be
            careful with your data, see our Privacy Policy.
          </div>
        </Grid>
        <Grid item xs={12} sm={6} md={5}>
          <nav className={styles.navigation}>
            <ul className={styles.navigationList}>
              {navigation.map((link, index) => (
                <li key={index} className={styles.navigationItem}>
                  <Link className={styles.navigationLink} to={link.link}>
                    {link.name}
                  </Link>
                </li>
              ))}
            </ul>
          </nav>
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <h3>Follow us</h3>
          <SocialMedia />
          <h3>Newsletter</h3>
          <NewsletterSignup />
        </Grid>
      </Grid>
    </Container>
  </div>
);
