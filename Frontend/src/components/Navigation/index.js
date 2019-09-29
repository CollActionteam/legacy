import React from "react";
import { Link } from "gatsby";
import { Button } from "../Button";
import { Hidden } from "@material-ui/core";
import styles from "./style.module.scss";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

export default class Navigation extends React.Component {

  constructor(props) {
    super(props);
      this.toggleNavigation = this.toggleNavigation.bind(this);
      this.state = {
        collapsed: true,
      };
  }

  toggleNavigation(){
    this.setState({
      collapsed: !this.state.collapsed
    });
  }

  render() {
    return (
      <div className={this.state.collapsed ? styles.collapsed : null}>
        <nav className={styles.navigation}>
          <ul className={styles.navigationList}>
            {this.props.items.map(link => (
              <li key={link.name} className={styles.navigationItem}>
                <Link className={styles.navigationLink} to={link.link}>
                  {link.name}
                </Link>
              </li>
            ))}
          </ul>
          <ul className={styles.navigationList}>
            <li className={styles.navigationItem}>
              <Link className={styles.donationLink} to="/donate">
                <FontAwesomeIcon icon="heart" />
                Donate
              </Link>
            </li>
            <li className={styles.navigationItem}>
              <Button to="/login">Login</Button>
            </li>
          </ul>
        </nav>
        <Hidden mdUp>
          <button className={styles.navigationToggle} onClick={this.toggleNavigation}>
            {this.state.collapsed ? <FontAwesomeIcon icon="times" /> : <FontAwesomeIcon icon="bars" /> }
          </button>
        </Hidden>
      </div>
    );
  }
};