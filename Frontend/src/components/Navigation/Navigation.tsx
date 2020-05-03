import React, { useState } from "react";
import { Hidden } from "@material-ui/core";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Link } from "react-router-dom";
import { useUser } from "../../providers/UserProvider";
import { Button } from "../../components/Button/Button";
import styles from "./Navigation.module.scss";

interface INavigationProps {
  items: { name: string, link: string }[];
}

export default ({ items } : INavigationProps) => {
  const [ collapsed, setCollapsed ] = useState(true);
  const toggleNavigation = () => setCollapsed(!collapsed);
  const user = useUser();

  const renderWithUser = () => {
    const returnUrl = window.location.href;
    const logoutUrl = `${process.env.REACT_APP_BACKEND_URL}/account/logout`;
    return <>
        <li className={styles.navigationItem}>
          <Link className={styles.navigationButton} to="/account/profile" onClick={() => setCollapsed(true)}>
            <span>Account</span>
          </Link>
        </li> 
        <li className={styles.navigationItem}>
          <form method="post" action={logoutUrl}>
            <input type="hidden" name="returnUrl" value={returnUrl} />
            <Button type="submit" className={styles.navigationSubmit}>
              <span>Logout</span>
            </Button>
          </form>
        </li> 
      </>;
  }

  const renderWithoutUser = () => {
    return <li className={styles.navigationItem}>
      <Link className={styles.navigationButton} to="/account/login" onClick={() => setCollapsed(true)}>
        <FontAwesomeIcon icon={["far", "user"]}/> 
        <span>Login</span>
      </Link>
    </li>;
  };

  return <div className={[styles.wrapper, collapsed ? styles.collapsed : ""].join(" ")}>
    <nav className={styles.navigation}>
      <ul className={styles.navigationList}>
        {items.map((link) => (
          <li key={link.name} className={styles.navigationItem}>
            <Link className={styles.navigationLink} to={link.link} onClick={() => setCollapsed(true)}>
              <span>{link.name}</span>
            </Link>
          </li>
        ))}
      </ul>
      <ul className={styles.navigationList}>
        <li className={styles.navigationItem}>
          <Link className={styles.navigationButton} to="/donate" onClick={() => setCollapsed(true)}>
            <FontAwesomeIcon icon={["far", "heart"]} />
            <span>Donate</span>
          </Link>
        </li>
        { user ? renderWithUser() : renderWithoutUser() }
      </ul>
    </nav>
    <Hidden mdUp>
      <button
        aria-label="Toggle navigation menu"
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