import * as React from "react";
import ProjectThumb from "./ProjectThumb";

export default ({ projectList, tileClassName, isEmbedded }) => {
  return (
    <div id="project-list">
      {projectList.map(project => <ProjectThumb key={project.projectId} tileClassName={tileClassName} isEmbedded={isEmbedded} {...project} />)}
    </div>
  );
};
