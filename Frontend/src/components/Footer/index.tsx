import React from "react";
import Container from "@material-ui/core/Container";
import Grid from "@material-ui/core/Grid";
import logo from "../../assets/svg/logo.svg";
import styles from "./style.module.scss";
import SocialMedia from "../SocialMedia";
import NewsletterSignup from "../NewsletterSignup";
import { Link } from "react-router-dom";
import { siteData } from "../../api/site";

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
            <a href="mailto:collactionteam@gmail.com">
              collactionteam@gmail.com
            </a>
            .<br />
            Would you like to get an occasional update on CollAction and
            crowdacting? Sign up for our newsletter (to your right)! We will be
            careful with your data, see our Privacy Policy.
          </div>
        </Grid>
        <Grid item xs={12} sm={6} md={5}>
          <nav className={styles.navigation}>
            <ul className={styles.navigationList}>
              {siteData.footerLinks.map((link: any, index: number) => (
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
          <h5 className={styles.title}>Follow us</h5>
          <SocialMedia socialMedia={siteData.socialMedia} />
          <h5 className={styles.title}>Newsletter</h5>
          <NewsletterSignup mailchimpListId={siteData.mailchimpListId} />
        </Grid>
      </Grid>
    </Container>
  </div>
);
