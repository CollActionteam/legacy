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
    name: "Home",
    link: "/",
  },
  {
    name: "Find Project",
    link: "/projects/find",
  },
  {
    name: "Start Project",
    link: "/projects/start",
  },
  {
    name: "About Us",
    link: "/about",
  },
  {
    name: "Login",
    link: "/login",
  },
  {
    name: "Sign Up",
    link: "/signup",
  },
  {
    name: "Mission",
    link: "/about",
  },
  {
    name: "Team",
    link: "/about",
  },
  {
    name: "Donate",
    link: "/donate",
  },
  {
    name: "Partners",
    link: "/about",
  },
  {
    name: "Press",
    link: "/about",
  },
  {
    name: "FAQs",
    link: "/about",
  },
];

export default () => (
  <div className={styles.footer}>
    <Container>
      <Link to="/">
        <img alt="CollAction" className={styles.logo} src={logo}></img>
      </Link>
      <Grid container>
        <Grid item md={4}>
          <p>
            Any questions, comments, or would you like to work together?
            Awesome! Email us at{" "}
            <Link to="mailto:collactionteam@gmail.com">
              collactionteam@gmail.com
            </Link>
            .<br />
            Would you like to get an occasional update on CollAction and
            crowdacting? Sign up for our newsletter (to your right)! We will be
            careful with your data, see our Privacy Policy.
          </p>
        </Grid>
        <Grid item md={5}>
          <nav className={styles.navigation}>
            <ul className={styles.navigationList}>
              {navigation.map(link => (
                <li className={styles.navigationItem}>
                  <Link className={styles.navigationLink} to={link.link}>
                    {link.name}
                  </Link>
                </li>
              ))}
            </ul>
          </nav>
        </Grid>
        <Grid item md={3}>
          <h3>Follow us</h3>
          <SocialMedia />
          <h3>Newsletter</h3>
          <NewsletterSignup />
        </Grid>
      </Grid>
    </Container>
  </div>
);
