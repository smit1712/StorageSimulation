function parseCommand(input = "") {
    return JSON.parse(input);
}

var Socket;
var plane;
var debug = false;

window.onload = function () {
    var camera, scene, renderer;
    var cameraControls;

    var worldObjects = {};

    function init() {
        camera = new THREE.PerspectiveCamera(70, window.innerWidth / window.innerHeight, 1, 1500);
        cameraControls = new THREE.OrbitControls(camera);
        camera.position.z = 0;
        camera.position.y = 20;
        camera.position.x = 50;
        cameraControls.update();
        scene = new THREE.Scene();

        renderer = new THREE.WebGLRenderer({ antialias: true });
        renderer.setPixelRatio(window.devicePixelRatio);
        renderer.setSize(window.innerWidth, window.innerHeight + 5);
        renderer.shadowMap.enabled = true;
        renderer.shadowMap.type = THREE.PCFShadowMap ; 
        document.body.appendChild(renderer.domElement);

      

        window.addEventListener('resize', onWindowResize, false);

        var geometry = new THREE.PlaneGeometry(30, 30, 30);
        var material = new THREE.MeshPhongMaterial({ map: new THREE.TextureLoader().load("textures/AmazonRoad.png"), side: THREE.DoubleSide });
        plane = new THREE.Mesh(geometry, material);
        plane.rotation.x = Math.PI / 2.0;
        plane.position.x = 15;
        plane.position.z = 15;
        plane.position.y = -0.1;
        plane.lights = true;
        plane.receiveShadow = true;
        scene.add(plane);


        if (!debug) {
            var SkyboxGeo = new THREE.SphereGeometry(1000, 32, 32);
            var SkyboxMat = new THREE.MeshLambertMaterial({ map: new THREE.TextureLoader().load("models/skybox/skybox.jpg"), side: THREE.DoubleSide });
            var skybox = new THREE.Mesh(SkyboxGeo, SkyboxMat);
            scene.add(skybox);
        }

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

        if (command.command === "update") {
            if (Object.keys(worldObjects).indexOf(command.parameters.guid) < 0) {
                // var loader = new THREE.ObjectLoader();
                // var fbxLoader = new THREE.FBXLoader();

                // Init Three Group
                var group = new THREE.Group();
                

                if (command.parameters.type === "robot") {
                    loadOBJModel("models/Drone/", "drone.obj", "models/Drone/", "drone.mtl", (obj) => {
                        obj.scale.set(5, 5, 5);    

                        var robotlight = new THREE.SpotLight(0xf44242, 0.5 ,4,0.50,0,0); 
                        robotlight.position = command.parameters.position;
                        robotlight.position.y += -1.5;                        
                        robotlight.target.position.set(15, -1000, 15);
                        robotlight.target.updateMatrixWorld();
                        robotlight.castShadow = true;
                        robotlight.shadow.mapSize.width = 4096;
                        robotlight.shadow.mapSize.height = 4096;
                        robotlight.shadow.camera.near = 0.5;
                        robotlight.shadow.camera.far = 1024; 
                        robotlight.shadow.camera = new THREE.OrthographicCamera(-10, 10, 10, -10, 0.5, 10); 

                        group.add(robotlight);   

                        obj.traverse(function (object) {
                        object.castShadow = true;
                        object.receiveShadow = true;
                        });
                        group.add(obj);
                    });
                } else if (command.parameters.type === "rack") {
                    loadOBJModel("models/rack/", "rack2.obj", "models/rack/", "rack2.mtl", (obj) => {
                        obj.scale.set(2, 2, 2);
                        obj.traverse(function (object) {
                            object.castShadow = true;
                            object.receiveShadow = true;
                        });             
                        group.add(obj);                        
                    });
                } else if (command.parameters.type === "transport") {
                    loadOBJModel("models/transport/", "xwing2.obj", "models/transport/", "xwing2.mtl", (obj) => {
                        obj.scale.set(10, 10, 10);
                        obj.traverse(function (object) {
                            object.castShadow = true;
                            object.receiveShadow = true;
                        });
                        group.add(obj);
                    });
                } else if (command.parameters.type === "node") {
                    var boxgeometry = new THREE.BoxGeometry(0.1, 10.0, 0.1);
                    var boxmaterial = new THREE.MeshBasicMaterial({ color: 0x00ff00 });
                    var cube = new THREE.Mesh(boxgeometry, boxmaterial);
                    group.add(cube);
                } else if (command.parameters.type === "adj") {
                    var adjgeometry = new THREE.BoxGeometry(0.1, 10.0, 0.1);
                    var adhmaterial = new THREE.MeshBasicMaterial({ color: 0xf44242 });
                    var adjcube = new THREE.Mesh(adjgeometry, adhmaterial);
                    group.add(adjcube);
                } else if (command.parameters.type === "sun") {
                    var sungeometry = new THREE.SphereGeometry(100, 100, 100 );
                    var sunmaterial = new THREE.MeshBasicMaterial({ color: 0xe5f442 });                    
                    var sunsphere = new THREE.Mesh(sungeometry, sunmaterial);
                    var sunlight = new THREE.DirectionalLight(0xffffff, 1);
                    sunlight.position = command.parameters.position;
                    if (!debug) {
                        sunlight.castShadow = true;
                        sunlight.shadow.mapSize.width = 4096;
                        sunlight.shadow.mapSize.height = 4096;
                        sunlight.shadow.camera.near = 0.5;    
                        sunlight.shadow.camera.far = 1024;                    
                        sunlight.shadow.camera = new THREE.OrthographicCamera(-100, 100, 100, -100, 0.5, 1000); 

                    }

                    group.add(sunsphere);
                    group.add(sunlight);
                    
                    
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

            object.visible = command.parameters.visible;
        }

        function loadOBJModel(modelPath, modelName, texturePath, textureName, onload) {
            new THREE.MTLLoader()
                .setPath(texturePath)
                .load(textureName, function (materials) {     
                    materials.color;
                    materials.preload();

                    new THREE.OBJLoader()
                        .setPath(modelPath)
                        .setMaterials(materials)
                        .load(modelName, function (object) {
                            onload(object);
                        },

                        // Progress %
                        function () { },

                        // Error
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