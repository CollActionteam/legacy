import React, { useState } from "react";
import { Hidden } from "@material-ui/core";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Link } from "react-router-dom";
import { useUser } from "../../providers/User";
import { Button } from "../../components/Button/Button";
import { IUser } from "../../api/types";
import styles from "./Navigation.module.scss";

interface INavigationProps {
  items: { name: string, link: string }[];
}

export default ({ items } : INavigationProps) => {
  const [ collapsed, setCollapsed ] = useState(true);
  const toggleNavigation = () => setCollapsed(!collapsed);
  const user = useUser();

  const renderWithUser = (user: IUser) => {
    const returnUrl = window.location.href;
    const logoutUrl = `${process.env.REACT_APP_BACKEND_URL}/account/logout`;
    return <React.Fragment>
        <li className={styles.navigationItem}>
          <Link className={styles.navigationButton} to="/account/profile">
            Profile
          </Link>
        </li> 
        <li className={styles.navigationItem}>
          <form method="post" action={logoutUrl}>
            <input type="hidden" name="returnUrl" value={returnUrl} />
            <Button type="submit" className={styles.navigationSubmit}>
              Logout
            </Button>
          </form>
        </li> 
      </React.Fragment>;
  }

  const renderWithoutUser = () => {
    return <li className={styles.navigationItem}>
      <Link className={styles.navigationButton} to="/account/login">
        Login
      </Link>
    </li>;
  };

  return <div className={collapsed ? styles.collapsed : ""}>
    <nav className={styles.navigation}>
      <ul className={styles.navigationList}>
        {items.map((link) => (
          <li key={link.name} className={styles.navigationItem}>
            <Link className={styles.navigationLink} to={link.link}>
              {link.name}
            </Link>
          </li>
        ))}
      </ul>
      <ul className={styles.navigationList}>
        <li className={styles.navigationItem}>
          <Link className={styles.navigationButton} to="/donate">
            <FontAwesomeIcon icon="heart" />
            Donate
          </Link>
        </li>
        { user ? renderWithUser(user) : renderWithoutUser() }
      </ul>
    </nav>
    <Hidden mdUp>
      <button
        className={styles.navigationToggle}
        onClick={toggleNavigation}
      >
        {collapsed ? (
          <FontAwesomeIcon icon="bars" />
        ) : (
          <FontAwesomeIcon icon="times" />
        )}
      </button>
    </Hidden>
  </div>;
};