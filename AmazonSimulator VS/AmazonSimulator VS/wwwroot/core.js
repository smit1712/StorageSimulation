function parseCommand(input = "") {
    return JSON.parse(input);
}

var Socket;

window.onload = function () {
    var camera, scene, renderer;
    var cameraControls;

    var worldObjects = {};

    function init() {
        camera = new THREE.PerspectiveCamera(70, window.innerWidth / window.innerHeight, 1, 1000);
        cameraControls = new THREE.OrbitControls(camera);
        camera.position.z = 15;
        camera.position.y = 5;
        camera.position.x = 15;
        cameraControls.update();
        scene = new THREE.Scene();

        renderer = new THREE.WebGLRenderer({ antialias: true });
        renderer.setPixelRatio(window.devicePixelRatio);
        renderer.setSize(window.innerWidth, window.innerHeight + 5);
        document.body.appendChild(renderer.domElement);

        window.addEventListener('resize', onWindowResize, false);

        var geometry = new THREE.PlaneGeometry(30, 30, 32);
        var material = new THREE.MeshBasicMaterial({ color: 0xffffff, side: THREE.DoubleSide });
        var plane = new THREE.Mesh(geometry, material);
        plane.rotation.x = Math.PI / 2.0;
        plane.position.x = 15;
        plane.position.z = 15;
        scene.add(plane);

        var light = new THREE.AmbientLight(0x404040);
        light.intensity = 4;
        scene.add(light);
    }

    function onWindowResize() {
        camera.aspect = window.innerWidth / window.innerHeight;
        camera.updateProjectionMatrix();
        renderer.setSize(window.innerWidth, window.innerHeight);
    }

    function animate() {
        requestAnimationFrame(animate);
        cameraControls.update();
        renderer.render(scene, camera);
    }

    Socket = new WebSocket("ws://" + window.location.hostname + ":" + window.location.port + "/connect_client");
    Socket.onmessage = function (event) {
        var command = parseCommand(event.data);
        var loader = new THREE.ObjectLoader();
        var fbxLoader = new THREE.FBXLoader();

        if (command.command === "update") {
            if (Object.keys(worldObjects).indexOf(command.parameters.guid) < 0) {
                // Init Three Group
                var group = new THREE.Group();

                if (command.parameters.type === "robot") {
                    var geometry = new THREE.BoxGeometry(0.9, 0.3, 0.9);
                    var cubeMaterials = [
                        new THREE.MeshBasicMaterial({ map: new THREE.TextureLoader().load("textures/robot_side.png"), side: THREE.DoubleSide }), //LEFT
                        new THREE.MeshBasicMaterial({ map: new THREE.TextureLoader().load("textures/robot_side.png"), side: THREE.DoubleSide }), //RIGHT
                        new THREE.MeshBasicMaterial({ map: new THREE.TextureLoader().load("textures/robot_top.png"), side: THREE.DoubleSide }), //TOP
                        new THREE.MeshBasicMaterial({ map: new THREE.TextureLoader().load("textures/robot_bottom.png"), side: THREE.DoubleSide }), //BOTTOM
                        new THREE.MeshBasicMaterial({ map: new THREE.TextureLoader().load("textures/robot_front.png"), side: THREE.DoubleSide }), //FRONT
                        new THREE.MeshBasicMaterial({ map: new THREE.TextureLoader().load("textures/robot_front.png"), side: THREE.DoubleSide }), //BACK
                    ];
                    var material = new THREE.MeshFaceMaterial(cubeMaterials);
                    var robot = new THREE.Mesh(geometry, material);
                    robot.position.y = 0.15;
                    // Add object to group
                    group.add(robot);
                } else if (command.parameters.type === "rack") {
                    loadOBJModel("models/rack/", "rack.obj", "models/rack/", "rack.mtl", (obj) => {       
                        obj.scale.set(0.03, 0.03, 0.03);
                        group.add(obj);
                    });
                } else if (command.parameters.type === "person") {
                    //fbxLoader.load(
                    //    "models3d/man.fbx",

                    //    function (obj) {
                    //        group.add(obj);
                    //    },

                    //    // called when loading is in progresses
                    //    function (xhr) {
                    //        //console.log((xhr.loaded / xhr.total * 100) + '% loaded');
                    //    },

                    //    // called when loading has errors
                    //    function (error) {
                    //        console.log('An error happened');
                    //        console.log(error);
                    //    }
                    //);
                } else if (command.parameters.type === "transport") {
                    loadOBJModel("models/transport/", "CUPIC_TRUCK.obj", "models/transport/", "CUPIC_TRUCK.mtl", (obj) => {
                        obj.scale.set(0.03, 0.03, 0.03);

                        group.add(obj);
                    });
                }

                // Add group to Scene
                scene.add(group);
                worldObjects[command.parameters.guid] = group;

            }

            var object = worldObjects[command.parameters.guid];

            object.position.x = command.parameters.x;
            object.position.y = command.parameters.y;
            object.position.z = command.parameters.z;

            object.rotation.x = command.parameters.rotationX;
            object.rotation.y = command.parameters.rotationY;
            object.rotation.z = command.parameters.rotationZ;
        }

        function loadOBJModel(modelPath, modelName, texturePath, textureName, onload) {
            new THREE.MTLLoader()
                .setPath(texturePath)
                .load(textureName, function (materials) {     
                    materials.color
                    materials.preload();

                    new THREE.OBJLoader()
                        .setPath(modelPath)
                        .setMaterials(materials)
                        .load(modelName, function (object) {
                            onload(object);
                        },

                        // Progress (%) handling
                        function () { },

                        // Error handling
                        function (e) {
                            console.log("Error loading model");
                            console.log(e);
                        });
                });
        }
    }

    init();
    animate();
}


// Spare code

//loader.load(
//     resource URL
//    "models3d/drone.json",

//     called when resource is loaded
//    function (obj) {
//        group.add(obj); // Add obj to group
//    },

//     called when loading is in progresses
//    function (xhr) {
//        console.log((xhr.loaded / xhr.total * 100) + '% loaded');
//    },

//     called when loading has errors
//    function (error) {
//        console.log('An error happened');
//    }
//);