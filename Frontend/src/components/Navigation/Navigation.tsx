import React, { useState } from 'react';
import { Hidden } from '@material-ui/core';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { Link } from 'react-router-dom';
import styles from './Navigation.module.scss';

interface INavigationProps {
  items: { name: string; link: string }[];
}

const Navigation = ({ items }: INavigationProps) => {
  const [collapsed, setCollapsed] = useState(true);
  const toggleNavigation = () => setCollapsed(!collapsed);

  return (
    <div
      className={[styles.wrapper, collapsed ? styles.collapsed : ''].join(' ')}
    >
      <nav className={styles.navigation}>
        <ul className={styles.navigationList}>
          {items.map((link) => (
            <li key={link.name} className={styles.navigationItem}>
              <Link
                className={styles.navigationLink}
                to={link.link}
                onClick={() => setCollapsed(true)}
              >
                <span>{link.name}</span>
              </Link>
            </li>
          ))}
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
    </div>
  );
};

export default Navigation;