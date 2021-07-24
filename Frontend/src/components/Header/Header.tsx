import React from 'react';
import { Link } from 'react-router-dom';
import Container from '@material-ui/core/Container';
import { siteData } from '../../api/site';
import logo from '../../assets/svg/logo.svg';
import { useUser } from '../../providers/UserProvider';
import { Button } from '../../components/Button/Button';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

import Navigation from '../Navigation/Navigation';

import styles from './Header.module.scss';

const Header = () => {
  const user = useUser();

  const renderWithoutUser = () => {
    return (
      <li className={styles.subnavigationListItem}>
        <Link className={styles.subnavigationLink} to="/account/login">
          <FontAwesomeIcon icon={['far', 'user']} />
          <span>Login</span>
        </Link>
      </li>
    );
  };

  const renderWithUser = () => {
    const returnUrl = window.location.href;
    const logoutUrl = `${process.env.REACT_APP_BACKEND_URL}/account/logout`;
    return (
      <>
        <li className={styles.subnavigationListItem}>
          <Link className={styles.subnavigationLink} to="/account/profile">
            <FontAwesomeIcon icon={['far', 'user']} />
            <span>Account</span>
          </Link>
        </li>
        <li className={styles.subnavigationListItem}>
          <form method="post" action={logoutUrl}>
            <input type="hidden" name="returnUrl" value={returnUrl} />
            <Button type="submit" className={styles.subnavigationSubmit}>
              <span>Logout</span>
            </Button>
          </form>
        </li>
      </>
    );
  };

  return (
    <div className={styles.header}>
      <Container>
        <div className={styles.wrapper}>
          <div className={styles.logoWrapper}>
            <Link to="/">
              <img alt="CollAction" className={styles.logo} src={logo}></img>
            </Link>
          </div>
          <div className={styles.navigationWrapper}>
            <Navigation items={siteData.menuLinks} />
            <nav className={styles.subnavigation}>
              <ul className={styles.subnavigationList}>
                <li className={styles.subnavigationListItem}>
                  <Link className={styles.subnavigationLink} to="/donate">
                    <FontAwesomeIcon icon={['far', 'heart']} />
                    <span>Donate</span>
                  </Link>
                </li>
                {user ? renderWithUser() : renderWithoutUser()}
              </ul>
            </nav>
          </div>
        </div>
      </Container>
    </div>
  );
};

export default Header;