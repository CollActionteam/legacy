import * as React from "react";
import ProjectThumb from "./ProjectThumb";

export default ({ projectList }) => {
  return (
    <div id="project-list">
      <div className="container">
        { projectList.map(project => <ProjectThumb key={project.projectId} {...project} />) }
      </div>
    </div>
  );
};
