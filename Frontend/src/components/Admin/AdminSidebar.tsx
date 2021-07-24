import { makeStyles, Drawer, Divider, List, ListItem, ListItemIcon, ListItemText } from "@material-ui/core";
import { useHistory } from "react-router-dom";
import React from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

const drawerWidth = 240;

const useStyles = makeStyles(theme => ({
  root: {
    display: 'flex',
  },
  drawer: {
    width: drawerWidth,
    flexShrink: 0,
  },
  drawerPaper: {
    width: drawerWidth,
  },
  content: {
    flexGrow: 1,
    backgroundColor: theme.palette.background.default,
    padding: theme.spacing(3),
  },
  toolbar: theme.mixins.toolbar,
  table: {
    minWidth: 650,
  }
}))

const AdminSidebar = ({ children }: any) => {
    const classes = useStyles();
    const history = useHistory();
    return <div className={classes.root}>
      <Drawer className={classes.drawer} variant="permanent" classes={{ paper: classes.drawerPaper, }} anchor="left">
        <div className={classes.toolbar} />
        <Divider />
        <List>
          <ListItem button key="Crowdactions" onClick={() => history.push("/admin/crowdactions/list")}>
            <ListItemIcon><FontAwesomeIcon icon="tools" /></ListItemIcon>
            <ListItemText primary="Crowdactions" />
          </ListItem>
          <ListItem button key="Users" onClick={() => history.push("/admin/users/list")}>
            <ListItemIcon><FontAwesomeIcon icon={["far", "user"]} /></ListItemIcon>
            <ListItemText primary="Users" />
          </ListItem>
          <ListItem button key="Comments" onClick={() => history.push("/admin/comments/list")}>
            <ListItemIcon><FontAwesomeIcon icon={["far", "comments"]} /></ListItemIcon>
            <ListItemText primary="Comments" />
          </ListItem>
        </List>
      </Drawer>
      <main className={classes.content}>
          { children }
      </main>
    </div>;
};

export default AdminSidebar;