import React from "react";

import { Avatar } from "@material-ui/core";

import styles from "./ProjectStarter.module.scss";

export const ProjectStarter = ({user}: any) => {
  
  return (
    <>
      { user &&
        <div className={styles.projectStarter}>
          <div className={styles.avatarContainer}>
            <Avatar className={styles.avatar}>
              {user.firstName?.charAt(0)}
              {user.lastName?.charAt(0)}
            </Avatar>
          </div>
          <h3>{user.fullName}</h3>
          <p className={styles.projectStarterTitle}>Project starter</p>
        </div>      
      }
    </>
  );
}
